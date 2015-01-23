using Newtonsoft.Json;
using shiwch.util;
using shiwch.util.FastMember;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shiwch.game
{
    public interface IResponseFormater
    {
        byte[] Formate(GameResponse response);
    }

    public class JsonFormater : IResponseFormater
    {
        byte[] IResponseFormater.Formate(GameResponse response)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
        }
    }
    public class ProtobufFormater : IResponseFormater
    {
        byte[] IResponseFormater.Formate(GameResponse response)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] buffer = NetworkBytes.ToBytes(response.ActionId);
                stream.Write(buffer, 0, buffer.Length);

                buffer = NetworkBytes.ToBytes(response.StatusCode);
                stream.Write(buffer, 0, buffer.Length);

                buffer = NetworkBytes.ToBytes(response.ReplyTo);
                stream.Write(buffer, 0, buffer.Length);

                buffer = NetworkBytes.ToBytes(response.Description);
                byte[] lenbuffer = NetworkBytes.ToBytes(buffer.Length);
                stream.Write(lenbuffer, 0, lenbuffer.Length);
                stream.Write(buffer, 0, buffer.Length);

                buffer = NetworkBytes.ToBytes(response.ResponseBody.Count);
                stream.Write(buffer, 0, buffer.Length);

                foreach (var segment in response.ResponseBody)
                {
                    var tmpbytes = new byte[4];
                    stream.Write(tmpbytes, 0, 4);
                    var pos1 = stream.Position;
                    ProtoBuf.Serializer.Serialize(stream, response.ResponseBody);
                    var pos2 = stream.Position;
                    stream.Seek(pos1 - 4, SeekOrigin.Begin);
                    buffer = NetworkBytes.ToBytes(pos2 - pos1);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Seek(0, SeekOrigin.End);
                }

                return stream.ToArray();
            }
        }
    }

    public class LegacyFormater : IResponseFormater
    {
        private MemoryStream outputStream;
        byte[] IResponseFormater.Formate(GameResponse response)
        {
            using (outputStream = new MemoryStream())
            {
                int len = 0;
                long pos = outputStream.Position;
                WriteValue(len);
                WriteValue(response.ActionId);
                WriteValue(response.StatusCode);
                WriteValue(response.ReplyTo);
                WriteValue(response.Description);
                foreach (var segment in response.ResponseBody)
                {
                    WriteObject(segment, false);
                }
                outputStream.Seek(pos, SeekOrigin.Begin);
                WriteValue((int)outputStream.Length);
                return outputStream.ToArray();
            }
        }

        private void WriteValue(byte value)
        {
            outputStream.WriteByte(value);
        }
        private void WriteValue(short value)
        {
            byte[] buf = BitConverter.GetBytes(value);
            outputStream.Write(buf, 0, buf.Length);
        }
        private void WriteValue(int value)
        {
            byte[] buf = BitConverter.GetBytes(value);
            outputStream.Write(buf, 0, buf.Length);
        }
        private void WriteValue(long value)
        {
            byte[] buf = BitConverter.GetBytes(value);
            outputStream.Write(buf, 0, buf.Length);
        }
        private void WriteValue(DateTime value)
        {
            var v = DateTimeExt.ToUnixTime(value);
            WriteValue(v);
        }
        private void WriteValue(float value)
        {
            byte[] buf = BitConverter.GetBytes(value);
            outputStream.Write(buf, 0, buf.Length);
        }
        private void WriteValue(string value)
        {
            byte[] buf = Encoding.UTF8.GetBytes(value);
            WriteValue(buf.Length);
            outputStream.Write(buf, 0, buf.Length);
        }
        private void WriteObject(object value, bool isInArray)
        {
            if (value == null)
            {
                WriteValue((int)0);
                return;
            }
            Type type = value.GetType();
            TypeCode typecode = Type.GetTypeCode(type);

            switch (typecode)
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                    WriteValue(Convert.ToByte(value));
                    break;
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.Single:
                    WriteValue(Convert.ToSingle(value));
                    break;
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    WriteValue(Convert.ToInt16(value));
                    break;
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    WriteValue(Convert.ToInt32(value));
                    break;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    WriteValue(Convert.ToInt64(value));
                    break;
                case TypeCode.String:
                    WriteValue((string)value);
                    break;
                case TypeCode.DateTime:
                    WriteValue((DateTime)value);
                    break;
                case TypeCode.Object:
                    if (value is IList)
                    {
                        var realvalue = value as IList;
                        WriteValue(realvalue.Count);
                        if (realvalue.Count != 0)
                        {
                            foreach (var inst in realvalue)
                            {
                                WriteObject(inst, true);
                            }
                        }
                    }
                    else
                    {
                        if (!isInArray) WriteValue((int)1);
                        long startPos = outputStream.Position;
                        WriteValue((int)0);
                        ObjectAccessor accessor = ObjectAccessor.Create(value);
                        foreach (var p in GetDataMembers(type))
                        {
                            WriteObject(accessor[p], false);
                        }
                        long endPos = outputStream.Position;

                        outputStream.Seek(startPos, SeekOrigin.Begin);
                        WriteValue((int)(endPos - startPos));
                        outputStream.Seek(endPos, SeekOrigin.Begin);
                    }
                    break;
            }
        }
        private static ConcurrentDictionary<Type, string[]> typeMembersCache = new ConcurrentDictionary<Type, string[]>();
        private static Type tagAtt = typeof(TagAttribute);
        private static string[] GetDataMembers(Type type)
        {
            string[] members;
            if (typeMembersCache.TryGetValue(type, out members)) return members;

            List<Tuple<string, int>> result = new List<Tuple<string, int>>();
            foreach (var p in type.GetProperties())
            {
                var attrs = p.GetCustomAttributes(tagAtt, true);
                if (attrs.Length == 1)
                {
                    var attr = attrs[0] as TagAttribute;
                    result.Add(new Tuple<string, int>(p.Name, attr.Tag));
                }
            }
            members = result.OrderBy(p => p.Item2).Select(p => p.Item1).ToArray();
            typeMembersCache[type] = members;
            return members;
        }
    }
}
