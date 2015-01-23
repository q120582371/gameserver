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
    /// 位操作助手类
    /// </summary>
    public static class BitUtil
    {
        /// <summary>
        /// 设置位
        /// </summary>
        /// <param name="value">要操作的数值</param>
        /// <param name="pos">要设置的位，最低位索引是0</param>
        /// <returns></returns>
        public static ulong SetBit(ulong value, int pos)
        {
            return value | (1UL << pos);
        }
        /// <summary>
        /// 判断位是否已设置
        /// </summary>
        /// <param name="value">要判断的数值</param>
        /// <param name="pos">要判断的位，最低位索引是0</param>
        /// <returns></returns>
        public static bool IsSet(ulong value, int pos)
        {
            return (value & (1UL << pos)) != 0;
        }
        /// <summary>
        /// 清除位
        /// </summary>
        /// <param name="value">要操作的数值</param>
        /// <param name="pos">要清除的位，最低位索引是0</param>
        /// <returns></returns>
        public static ulong ClearBit(ulong value, int pos)
        {
            return value & (~(1UL << pos));
        }

        /// <summary>
        /// 反转某位
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static ulong Invert(ulong value, int pos)
        {
            return value ^ (1UL << pos);
        }
    }
}
