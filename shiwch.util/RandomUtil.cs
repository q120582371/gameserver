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
    public static class RandomUtil
    {
        #region 根据概率获取位置索引
        private static object samplingLock = new object();
        private static Random samplingRandom = new Random();
        static double[] pdf2cdf(double[] pdf)
        {
            double[] cdf = new double[pdf.Length];
            Array.Copy(pdf, cdf, pdf.Length);
            for (int i = 1; i < pdf.Length - 1; i++)
                cdf[i] += cdf[i - 1];

            cdf[cdf.Length - 1] = 1;
            return cdf;
        }
        /// <summary>
        /// 根据概率获取对象
        /// </summary>
        /// <param name="list">对象列表</param>
        /// <param name="pdf">概率数组，1表示100%，如果概率数组总和不等于1的情况下，有可能返回null，表示在所有的概率之外</param>
        /// <returns></returns>
        public static T GetIndex<T>(List<T> list, double[] pdf)
        {
            var idx = GetIndex(pdf);
            if (idx == -1) return default(T);
            return list[idx];
        }
        /// <summary>
        /// 根据概率获取位置索引
        /// </summary>
        /// <param name="pdf">概率数组，1表示100%，如果概率数组总和不等于1的情况下，有可能返回-1，表示在所有的概率之外</param>
        /// <returns></returns>
        public static int GetIndex(double[] pdf)
        {
            var cdf = pdf2cdf(pdf);
            double f;
            lock (samplingLock)
            {
                f = samplingRandom.NextDouble();
            }

            for (int i = 0; i < cdf.Length; i++)
            {
                if (f < cdf[i]) return i;
            }

            return -1;
        }
        /// <summary>
        /// 是否命中某个概率
        /// </summary>
        /// <param name="rate">概率，1表示100%</param>
        /// <returns></returns>
        public static bool IsHit(double rate)
        {
            return GetIndex(new double[] { rate, 1 - rate }) == 0;
        }
        #endregion

        #region 打乱一个列表
        /// <summary>
        /// 打乱一个列表
        /// </summary>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            Random rng = new Random();
            T[] elements = source.ToArray();
            if (elements.Length == 0) yield break;
            for (int i = elements.Length - 1; i > 0; i--)
            {
                int swapIndex = rng.Next(i + 1);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }

            yield return elements[0];
        }
        #endregion
    }
}
