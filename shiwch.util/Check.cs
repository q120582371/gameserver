// -------------------------------------------------------
// Copyright (C) 施维串 版权所有。
// 创建标识：2013-11-11 10:53:36 Created by 施维串
// 功能说明：
// 注意事项：
// 
// 更新记录：
// -------------------------------------------------------



using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shiwch.util
{
    /// <summary>
    /// 对象空以及空字符串断言
    /// </summary>
    public static class Check
    {
        /// <summary>
        /// 判断对象T非空，如果为空则抛出ArgumentNullException异常
        /// </summary>
        /// <param name="value">对象</param>
        /// <param name="parameterName">参数名</param>
        public static void NotNull<T>(T value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// 判断对象T非空，如果为空则抛出ArgumentNullException异常
        /// </summary>
        /// <param name="value">对象</param>
        /// <param name="parameterName">参数名</param>
        //public static void NotNull(object value, string parameterName)
        //{
        //    if (value == null)
        //    {
        //        throw new ArgumentNullException(parameterName);
        //    }
        //}

        /// <summary>
        /// 判断字符串非空，如果为空则抛出ArgumentException异常
        /// </summary>
        /// <param name="value">字符串对象</param>
        /// <param name="parameterName">参数名</param>
        public static void NotEmpty(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("The argument cannot be null or empty.", parameterName);
            }
        }

        /// <summary>
        /// 判断数组对象非空，如果为空则抛出ArgumentNullException异常
        /// 判断数组成员非空，如果为空则抛出ArgumentException异常
        /// </summary>
        /// <param name="array">数组对象</param>
        /// <param name="parameterName">参数名</param>
        public static void ArrayNotNull<T>(IEnumerable<T> array, string parameterName)
        {
            if (array == null) throw new ArgumentNullException(parameterName);
            foreach (var e in array)
            {
                if (e == null) throw new ArgumentException("The array members cannot be null.", parameterName);
            }
        }

        /// <summary>
        /// 判断字符串数组对象非空，如果为空则抛出ArgumentNullException异常
        /// 判断字符串数组成员非空和非空白字符，如果为空或者空白字符则抛出ArgumentException异常
        /// </summary>
        /// <param name="array">数组对象</param>
        /// <param name="parameterName">参数名</param>
        public static void ArrayNotEmpty(IEnumerable<string> array, string parameterName)
        {
            if (array == null) throw new ArgumentNullException(parameterName);
            foreach (var e in array)
            {
                if (string.IsNullOrEmpty(e)) throw new ArgumentException("The array members cannot be null or empty.", parameterName);
            }
        }
    }
}
