// -------------------------------------------------------
// Copyright (C) 施维串 版权所有。
// 创建标识：2013-11-11 10:53:36 Created by 施维串
// 功能说明：
// 注意事项：
// 
// 更新记录：
// -------------------------------------------------------



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace shiwch.util
{
    /// <summary>
    /// 字节数组操作助手类
    /// </summary>
    public static class ByteExt
    {
        #region 连接字节数组
        /// <summary>
        /// 连接字节数组
        /// </summary>
        /// <param name="values">要连接的字节数组</param>
        /// <returns>连接后的字节数组</returns>
        /// <exception cref="ArgumentNullException">values为空</exception>
        public static byte[] Concat(params byte[][] values)
        {
            Check.NotNull(values, "values");
            byte[] r = new byte[values.Sum(p => p == null ? 0 : p.Length)];
            int idx = 0;
            foreach (var b in values)
            {
                if (b != null && b.Length != 0)
                {
                    Array.Copy(b, 0, r, idx, b.Length);
                    idx += b.Length;
                }
            }
            return r;
        }
        /// <summary>
        /// 连接字节数组
        /// </summary>
        /// <param name="b1">要连接的字节数组1</param>
        /// <param name="values">其他要连接的字节数组</param>
        /// <returns>连接后的字节数组</returns>
        /// <exception cref="ArgumentNullException">b1为空</exception>
        public static byte[] Concat(this byte[] b1, params byte[][] values)
        {
            Check.NotNull(values, "values");
            byte[][] r = new byte[values.Length + 1][];
            r[0] = b1;
            Array.Copy(values, 0, r, 1, values.Length);
            return Concat(r);
        }
        #endregion

        #region 检索子字节数组
        /// <summary>
        /// 检索子字节数组。子字节数组从指定的位置开始且具有指定的长度。
        /// </summary>
        /// <param name="bytes">要检索的字节数组</param>
        /// <param name="startIndex">子字节数组的起始位置（从零开始）。</param>
        /// <param name="length">子字节数组的字节数。</param>
        /// <returns>一个byte数组，它等于此实例中从 startIndex 开始的长度为 length 的子字符串，如果 startIndex 等于此实例的长度且 length 为零，则为零长度的byte数组。</returns>
        /// <exception cref="ArgumentOutOfRangeException">startIndex 或 length 小于零。</exception>
        /// <exception cref="ArgumentNullException">b为空</exception>
        /// <exception cref="ArgumentException">length 大于 startIndex 开始的字节数</exception>
        public static byte[] SubBytes(this byte[] bytes, int startIndex, int length)
        {
            Check.NotNull(bytes, "bytes");
            if (startIndex < 0 || length < 0)
            {
                throw new ArgumentOutOfRangeException((startIndex < 0) ? "startIndex" : "length", "不能小于零。");
            }
            if (bytes.Length - startIndex < length)
            {
                throw new ArgumentException("length 大于 startIndex 开始的字节数。");
            }
            if (length == 0) return new byte[0];
            byte[] r = new byte[length];
            Array.Copy(bytes, startIndex, r, 0, length);
            return r;
        }
        #endregion

        #region 16进制表示
        /// <summary>
        /// 字节数组的字符串表现形式
        /// </summary>
        /// <param name="bytes">要转换的字节数组</param>
        /// <returns></returns>
        public static string ToHex(this byte[] bytes)
        {
            Check.NotNull(bytes, "bytes");

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
        /// <summary>
        /// 字节数组的字符串表现形式
        /// </summary>
        /// <param name="bytes">要转换的字节数组</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">字节数</param>
        /// <returns></returns>
        public static string ToHex(this byte[] bytes, int startIndex, int count)
        {
            Check.NotNull(bytes, "bytes");
            if (startIndex < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException((startIndex < 0) ? "startIndex" : "length", "不能小于零。");
            }
            if (bytes.Length - startIndex < count)
            {
                throw new ArgumentException("length 大于 startIndex 开始的字节数。");
            }
            if (count == 0) return "";

            char[] c = new char[count * 2];

            byte b;

            for (int bx = startIndex, cx = 0; bx < startIndex + count; ++bx, ++cx)
            {
                b = ((byte)(bytes[bx] >> 4));
                c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);

                b = ((byte)(bytes[bx] & 0x0F));
                c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
            }

            return new string(c);
        }
        #endregion
    }
}
