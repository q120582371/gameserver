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

namespace shiwch.util
{
    /// <summary>
    /// 字节数组相等比较器
    /// </summary>
    public sealed class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
    {
        /// <summary>
        /// 判断两个字节数组是否相等
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public bool Equals(byte[] first, byte[] second)
        {
            return ByteEquals(first, second);
        }

        /// <summary>
        /// 判断两个字节数组是否相等
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool ByteEquals(byte[] first, byte[] second)
        {
            if (first == second)
            {
                return true;
            }
            if (first == null || second == null)
            {
                return false;
            }
            if (first.Length != second.Length)
            {
                return false;
            }
            for (int i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(byte[] array)
        {
            if (array == null)
            {
                return 0;
            }
            int num = 17;
            for (int i = 0; i < array.Length; i++)
            {
                byte b = array[i];
                num = num * 31 + (int)b;
            }
            return num;
        }
    }
}
