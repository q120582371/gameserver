// -------------------------------------------------------
// Copyright (C) 施维串 版权所有。
// 创建标识：2013-11-11 10:53:36 Created by 施维串
// 功能说明：
// 注意事项：
// 
// 更新记录：
// -------------------------------------------------------


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace shiwch.util
{
    /// <summary>
    /// 网络字节序
    /// </summary>
    public static class NetworkBytes
    {
        /// <summary>
        /// 将对象转成字节数组的表现形式，基础类型转化为网络字节序的字节表示，对象类型使用MessagePack序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static byte[] ToBytes(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            var tempByteArray = obj as byte[];
            if (tempByteArray != null) return tempByteArray;
            var objType = obj.GetType();
            TypeCode code = Type.GetTypeCode(objType);
            switch (code)
            {
                case TypeCode.SByte: return ToBytes((sbyte)obj);
                case TypeCode.Byte: return ToBytes((byte)obj);
                case TypeCode.Char: return ToBytes((char)obj);
                case TypeCode.String: return ToBytes((string)obj);
                case TypeCode.Boolean: return ToBytes((bool)obj);
                case TypeCode.Int16: return ToBytes((short)obj);
                case TypeCode.Int32: return ToBytes((int)obj);
                case TypeCode.Int64: return ToBytes((long)obj);
                case TypeCode.UInt16: return ToBytes((ushort)obj);
                case TypeCode.UInt32: return ToBytes((uint)obj);
                case TypeCode.UInt64: return ToBytes((ulong)obj);
                case TypeCode.DateTime: return ToBytes((DateTime)obj);
                case TypeCode.Double: return ToBytes((double)obj);
                case TypeCode.Single: return ToBytes((float)obj);
                default:
                    using (MemoryStream stream = new MemoryStream())
                    {
                        ProtoBuf.Serializer.NonGeneric.Serialize(stream, obj);
                        return stream.ToArray();
                    }
            }
        }
        /// <summary>
        /// 将对象写入到流，基础类型转化为网络字节序的字节表示，对象类型使用protobuf序列化
        /// </summary>
        /// <param name="obj">要写入的对象</param>
        /// <param name="stream">流对象</param>
        public static void WriteToStream(object obj, Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (obj == null) throw new ArgumentNullException("obj");
            var tempByteArray = obj as byte[];
            if (tempByteArray != null)
            {
                stream.Write(tempByteArray, 0, tempByteArray.Length);
                return;
            }

            var objType = obj.GetType();
            TypeCode code = Type.GetTypeCode(objType);
            switch (code)
            {
                case TypeCode.SByte: { stream.Write(ToBytes((sbyte)obj), 0, 1); return; }
                case TypeCode.Byte: { stream.Write(ToBytes((byte)obj), 0, 1); return; }
                case TypeCode.Char:
                    {
                        var bytes = ToBytes((char)obj);
                        stream.Write(bytes, 0, bytes.Length);
                        return;
                    }
                case TypeCode.String:
                    {
                        var bytes = ToBytes((string)obj);
                        stream.Write(bytes, 0, bytes.Length);
                        return;
                    }
                case TypeCode.Boolean:
                    {
                        var bytes = ToBytes((bool)obj);
                        stream.Write(bytes, 0, bytes.Length);
                        return;
                    }
                case TypeCode.Int16:
                    {
                        var bytes = ToBytes((short)obj);
                        stream.Write(bytes, 0, bytes.Length);
                        return;
                    }
                case TypeCode.Int32:
                    {
                        var bytes = ToBytes((int)obj);
                        stream.Write(bytes, 0, bytes.Length);
                        return;
                    }
                case TypeCode.Int64:
                    {
                        var bytes = ToBytes((long)obj);
                        stream.Write(bytes, 0, bytes.Length);
                        return;
                    }
                case TypeCode.UInt16:
                    {
                        var bytes = ToBytes((ushort)obj);
                        stream.Write(bytes, 0, bytes.Length);
                        return;
                    }
                case TypeCode.UInt32:
                    {
                        var bytes = ToBytes((uint)obj);
                        stream.Write(bytes, 0, bytes.Length);
                        return;
                    }
                case TypeCode.UInt64:
                    {
                        var bytes = ToBytes((ulong)obj);
                        stream.Write(bytes, 0, bytes.Length);
                        return;
                    }
                case TypeCode.DateTime:
                    {
                        var bytes = ToBytes((DateTime)obj);
                        stream.Write(bytes, 0, bytes.Length);
                        return;
                    }
                case TypeCode.Double:
                    {
                        var bytes = ToBytes((double)obj);
                        stream.Write(bytes, 0, bytes.Length);
                        return;
                    }
                case TypeCode.Single:
                    {
                        var bytes = ToBytes((float)obj);
                        stream.Write(bytes, 0, bytes.Length);
                        return;
                    }
                default:
                    {
                        ProtoBuf.Serializer.NonGeneric.Serialize(stream, obj);
                        return;
                    }
            }
        }
        /// <summary>
        /// 从字节流转成对象
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="type">对象类型</param>
        /// <returns></returns>
        public static object AsObject(byte[] bytes, Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (bytes == null) throw new ArgumentNullException("bytes");
            if (type == typeof(byte[])) return bytes;

            TypeCode code = Type.GetTypeCode(type);
            switch (code)
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                    if (bytes.Length != 1) throw new ArgumentException("bytes长度必须等于1");
                    return bytes[0];
                case TypeCode.Char: return AsChar(bytes);
                case TypeCode.String: return AsString(bytes);
                case TypeCode.Boolean: return AsBoolean(bytes);
                case TypeCode.Int16: return AsInt16(bytes);
                case TypeCode.Int32: return AsInt32(bytes);
                case TypeCode.Int64: return AsInt64(bytes);
                case TypeCode.UInt16: return AsUInt16(bytes);
                case TypeCode.UInt32: return AsUInt32(bytes);
                case TypeCode.UInt64: return AsUInt64(bytes);
                case TypeCode.DateTime: return AsDateTime(bytes);
                case TypeCode.Double: return AsDouble(bytes);
                case TypeCode.Single: return AsSingle(bytes);
                default:
                    if (bytes.Length == 0) return null;
                    using (MemoryStream stream = new MemoryStream(bytes))
                    {
                        return ProtoBuf.Serializer.NonGeneric.Deserialize(type, stream);
                    }
            }
        }
        /// <summary>
        /// 从字节数组转成对象
        /// </summary>
        /// <typeparam name="T">字节数组</typeparam>
        /// <param name="bytes">对象类型</param>
        /// <returns></returns>
        public static T AsObject<T>(byte[] bytes)
        {
            return (T)AsObject(bytes, typeof(T));
        }

        #region ToBytes
        public static byte[] ToBytes(byte val)
        {
            return new byte[] { val };
        }
        public static byte[] ToBytes(sbyte val)
        {
            byte value = (byte)val;
            return ToBytes(value);
        }
        public static byte[] ToBytes(string val)
        {
            return Encoding.UTF8.GetBytes(val);
        }
        public static byte[] ToBytes(char val)
        {
            return ToBytes(val.ToString());
        }
        public static byte[] ToBytes(long val)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(val));
        }
        public static byte[] ToBytes(ulong val)
        {
            long value = (long)val;
            return ToBytes(value);
        }
        public static byte[] ToBytes(int val)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(val));
        }
        public static byte[] ToBytes(uint val)
        {
            int value = (int)val;
            return ToBytes(value);
        }
        public static byte[] ToBytes(short val)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(val));
        }
        public static byte[] ToBytes(ushort val)
        {
            short value = (short)val;
            return ToBytes(value);
        }
        public static byte[] ToBytes(bool val)
        {
            return new byte[] { val ? (byte)1 : (byte)0 };
        }
        public static byte[] ToBytes(float val)
        {
            return BitConverter.GetBytes(val);
        }
        public static byte[] ToBytes(double val)
        {
            return BitConverter.GetBytes(val);
        }
        public static byte[] ToBytes(DateTime val)
        {
            long value = val.ToJavaTimeMillis();
            return ToBytes(value);
        }
        #endregion

        #region FromBytes
        public static bool AsBoolean(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            if (bytes.Length != 1) throw new ArgumentException("bytes长度必须等于1");
            return bytes[0] != 0;
        }
        public static short AsInt16(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            if (bytes.Length != 2) throw new ArgumentException("bytes长度必须等于2");
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt16(bytes, 0));
        }
        public static int AsInt32(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            if (bytes.Length != 4) throw new ArgumentException("bytes长度必须等于4");
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 0));
        }
        public static long AsInt64(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            if (bytes.Length != 8) throw new ArgumentException("bytes长度必须等于8");
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt64(bytes, 0));
        }
        public static ushort AsUInt16(byte[] bytes)
        {
            short val = AsInt16(bytes);
            ushort result = (ushort)val;
            return result;
        }
        public static uint AsUInt32(byte[] bytes)
        {
            int val = AsInt32(bytes);
            uint result = (uint)val;
            return result;
        }
        public static ulong AsUInt64(byte[] bytes)
        {
            long val = AsInt64(bytes);
            ulong result = (ulong)val;
            return result;
        }
        public static string AsString(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            return Encoding.UTF8.GetString(bytes);
        }
        public static char AsChar(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            string val = AsString(bytes);
            return val == string.Empty ? ' ' : val[0];
        }
        public static DateTime AsDateTime(byte[] bytes)
        {
            long val = AsInt64(bytes);
            return val.FromJavaTimeMillisLocal();
        }
        public static double AsDouble(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            if (bytes.Length != 8) throw new ArgumentException("bytes长度必须等于8");
            return BitConverter.ToDouble(bytes, 0);
        }
        public static float AsSingle(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            if (bytes.Length != 4) throw new ArgumentException("bytes长度必须等于4");
            return BitConverter.ToSingle(bytes, 0);
        }
        #endregion
    }
}
