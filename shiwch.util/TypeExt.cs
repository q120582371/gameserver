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
    /// Type扩展方法
    /// </summary>
    public static class TypeExt
    {
        /// <summary>
        /// 判断指定类型是否为值类型
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <returns></returns>
        public static bool IsNumeric(this Type type)
        {
            if (type == null) return false;

            return type == typeof(Byte)
                || type == typeof(Int16)
                || type == typeof(Int32)
                || type == typeof(Int64)
                || type == typeof(SByte)
                || type == typeof(UInt16)
                || type == typeof(UInt32)
                || type == typeof(UInt64)
                || type == typeof(Decimal)
                || type == typeof(Double)
                || type == typeof(Single)
                || type.IsEnum;
        }
    }
}
