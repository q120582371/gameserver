using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace shiwch.game.net
{
    static class Util
    {
        public static byte[] MD5(byte[] source, int offset, int count)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                return md5.ComputeHash(source, offset, count);
            }
        }
        
        public static string ToHex(byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];

            byte b;

            for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx)
            {
                b = ((byte)(bytes[bx] >> 4));
                c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);

                b = ((byte)(bytes[bx] & 0x0F));
                c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
            }

            return new string(c);
        }
        
        public static bool ByteEquals(byte[] first, int offset, int count, byte[] second)
        {
            if (first == second)
            {
                return true;
            }
            if (first == null || second == null)
            {
                return false;
            }
            if (count != second.Length)
            {
                return false;
            }
            for (int i = 0; i < count; i++)
            {
                if (first[offset + i] != second[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static byte[] Concat(byte[] v1, int offset, int count, byte[] v2)
        {
            byte[] r = new byte[count + v2.Length];
            Array.Copy(v1, offset, r, 0, count);
            Array.Copy(v2, 0, r, count, v2.Length);
            return r;
        }

        private const long UnixEpoch = 621355968000000000L;
        
        public static int ToUnixTime(DateTime dateTime)
        {
            return (int)((dateTime.ToUniversalTime().Ticks - UnixEpoch) / TimeSpan.TicksPerSecond);
        }
    }
}
