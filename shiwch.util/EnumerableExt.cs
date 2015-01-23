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

namespace shiwch.util
{
    /// <summary>
    /// IEnumerable&lt;T&gt;的扩展方法
    /// </summary>
    public static class EnumerableExt
    {
        /// <summary>
        /// 对source中的每个元素执行action
        /// </summary>
        /// <typeparam name="T">source中的元素类型</typeparam>
        /// <param name="source">序列</param>
        /// <param name="action">要执行的Action</param>
        public static void Each<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            Check.NotNull(source, "source");
            Check.NotNull(action, "action");
            var i = 0;
            foreach (var e in source)
            {
                action(e, i++);
            }
        }

        /// <summary>
        /// 对source中的每个元素执行action
        /// </summary>
        /// <typeparam name="T">source中的元素类型</typeparam>
        /// <param name="source">序列</param>
        /// <param name="action">要执行的Action</param>
        public static void Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            Check.NotNull(source, "source");
            Check.NotNull(action, "action");
            foreach (var e in source)
            {
                action(e);
            }
        }

        /// <summary>
        /// 把source中的每个元素使用separator连接起来
        /// </summary>
        /// <typeparam name="T">source中的元素类型</typeparam>
        /// <param name="source">序列</param>
        /// <param name="selector">应用于每个元素上的选择器</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static string Join<T>(this IEnumerable<T> source, Func<T, string> selector = null, string separator = ", ")
        {
            Check.NotNull(source, "source");

            selector = selector ?? (t => t.ToString());

            return string.Join(separator, source.Where(t => !ReferenceEquals(t, null)).Select(selector));
        }

        /// <summary>
        /// 把一个元素value追加到序列的前面
        /// </summary>
        /// <typeparam name="T">source中的元素类型</typeparam>
        /// <param name="source">序列</param>
        /// <param name="value">要追加的value</param>
        /// <returns></returns>
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T value)
        {
            Check.NotNull(source, "source");

            yield return value;

            foreach (var element in source)
            {
                yield return element;
            }
        }

        /// <summary>
        /// 把一个元素value追加到序列的后面
        /// </summary>
        /// <typeparam name="T">source中的元素类型</typeparam>
        /// <param name="source">序列</param>
        /// <param name="value">要追加的value</param>
        /// <returns></returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T value)
        {
            Check.NotNull(source, "source");

            foreach (var element in source)
            {
                yield return element;
            }

            yield return value;
        }
    }
}
