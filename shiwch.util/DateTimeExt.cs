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
    /// DateTime助手类
    /// </summary>
    public static class DateTimeExt
    {
        /// <summary>
        /// unix时间原点
        /// </summary>
        public static readonly DateTime UnixTimeEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 将DateTime转成java的System.currentMillis()格式的时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToJavaTimeMillis(this DateTime dateTime)
        {
            return (long)((dateTime.ToUniversalTime() - UnixTimeEpoch).TotalMilliseconds);
        }

        /// <summary>
        /// 从java的System.currentMillis()格式的时间戳转成.Net的DateTime对象，UTC时间
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static DateTime FromJavaTimeMillisUTC(this long val)
        {
            return UnixTimeEpoch.AddMilliseconds(val);
        }

        /// <summary>
        /// 从java的System.currentMillis()格式的时间戳转成.Net的DateTime对象，本地时间
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static DateTime FromJavaTimeMillisLocal(this long val)
        {
            return FromJavaTimeMillisUTC(val).ToLocalTime();
        }

        /// <summary>
        /// 将DateTime转成Unix格式的时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int ToUnixTime(this DateTime dateTime)
        {
            return (int)((dateTime.ToUniversalTime() - UnixTimeEpoch).TotalSeconds);
        }

        /// <summary>
        /// 将Unix格式的时间戳转成.Net的DateTime对象，UTC时间
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static DateTime FromUnixTimeUTC(this int val)
        {
            return UnixTimeEpoch.AddSeconds(val);
        }

        /// <summary>
        /// 将Unix格式的时间戳转成.Net的DateTime对象，本地时间
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static DateTime FromUnixTimeLocal(this int val)
        {
            return FromUnixTimeUTC(val).ToLocalTime();
        }
    }
}
