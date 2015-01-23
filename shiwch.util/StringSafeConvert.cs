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
using System.Threading.Tasks;

namespace shiwch.util
{
    public static class StringSafeConvert
    {
        public static short ToInt16(this string value, short @default = 0)
        {
            if (string.IsNullOrEmpty(value)) return @default;

            short v;
            if (short.TryParse(value, out v)) return v;
            return @default;
        }

        public static short ToInt16(this object value, short @default = 0)
        {
            string strValue;
            if ((value == null) || (value == DBNull.Value) || ((strValue = value.ToString()).Length == 0)) return @default;

            return ToInt16(strValue, @default);
        }

        public static int ToInt32(this string value, int @default = 0)
        {
            if (string.IsNullOrEmpty(value)) return @default;

            int v;
            if (int.TryParse(value, out v)) return v;
            return @default;
        }

        public static int ToInt32(this object value, int @default = 0)
        {
            string strValue;
            if ((value == null) || (value == DBNull.Value) || ((strValue = value.ToString()).Length == 0)) return @default;

            return ToInt32(strValue, @default);
        }

        public static long ToInt64(this string value, long @default = 0)
        {
            if (string.IsNullOrEmpty(value)) return @default;

            long v;
            if (long.TryParse(value, out v)) return v;
            return @default;
        }

        public static long ToInt64(this object value, long @default = 0)
        {
            string strValue;
            if ((value == null) || (value == DBNull.Value) || ((strValue = value.ToString()).Length == 0)) return @default;

            return ToInt64(strValue, @default);
        }

        public static double ToDouble(this string value, double @default = 0)
        {
            if (string.IsNullOrEmpty(value)) return @default;

            double v;
            if (double.TryParse(value, out v)) return v;
            return @default;
        }

        public static double ToDouble(this object value, double @default = 0)
        {
            string strValue;
            if ((value == null) || (value == DBNull.Value) || ((strValue = value.ToString()).Length == 0)) return @default;

            return ToDouble(strValue, @default);
        }

        public static DateTime ToDateTime(this string value)
        {
            return ToDateTime(value, DateTimeExt.UnixTimeEpoch);
        }

        public static DateTime ToDateTime(this string value, DateTime @default)
        {
            if (string.IsNullOrEmpty(value)) return @default;

            DateTime v;
            if (DateTime.TryParse(value, out v)) return v;
            return @default;
        }

        public static DateTime ToDateTime(this object value)
        {
            return ToDateTime(value, DateTimeExt.UnixTimeEpoch);
        }

        public static DateTime ToDateTime(this object value, DateTime @default)
        {
            string strValue;
            if ((value == null) || (value == DBNull.Value) || ((strValue = value.ToString()).Length == 0)) return @default;

            return ToDateTime(strValue, @default);
        }


        public static bool ToBoolean(this string value, bool @default = false)
        {
            if (string.IsNullOrEmpty(value)) return @default;
            if (value == "1" || string.Compare(value, "true", true) == 0) return true;
            else if (value == "0" || string.Compare(value, "false", true) == 0) return false;
            else return @default;
        }

        public static bool ToBoolean(this object value, bool @default = false)
        {
            string strValue;
            if ((value == null) || (value == DBNull.Value) || ((strValue = value.ToString()).Length == 0)) return @default;

            return ToBoolean(strValue, @default);
        }
    }
}
