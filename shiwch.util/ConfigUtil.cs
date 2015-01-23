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
using System.Configuration;

namespace shiwch.util
{
    /// <summary>
    /// 配置文件助手类
    /// </summary>
    public class ConfigUtil
    {
        /// <summary>
        /// 获取数据库连接串
        /// </summary>
        /// <param name="key">配置名</param>
        /// <returns></returns>
        public static string GetDbConnStr(string key)
        {
            var str = Get<string>(key);
            return CryptHelper.DES_Decrypt(str);
        }
        /// <summary>
        /// 获取配置，如果指定key的配置不存在，那么抛出异常
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="key">配置名称</param>
        /// <exception cref="ArgumentNullException">key为空或者为空字符串</exception>
        /// <exception cref="ArgumentException">配置项key未进行配置</exception>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            Check.NotEmpty(key, "key");
            var v = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(v)) throw new ArgumentException(string.Format("Config item '{0}' not exist in config file.", key));

            return ChangeType<T>(v);
        }
        /// <summary>
        /// 获取配置，如果指定key的配置不存在，那么返回默认值
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="key">配置名称</param>
        /// <param name="default">默认值</param>
        /// <exception cref="ArgumentNullException">key为空或者为空字符串</exception>
        /// <returns></returns>
        public static T Get<T>(string key, T @default)
        {
            Check.NotEmpty(key, "key");
            var v = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(v)) return @default;
            return ChangeType<T>(v);
        }

        /// <summary>
        /// 把字符串转成指定的类型，简单类型通过Convert.ChangeType进行转换，object类型通过json反序列化进行转换
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="value">要进行转换的对象的字符串表现形式</param>
        /// <exception cref="ArgumentNullException">value为空或者为空字符串</exception>
        /// <exception cref="InvalidCastException">value无法转成目标类型</exception>
        /// <returns></returns>
        public static T ChangeType<T>(string value)
        {
            Check.NotEmpty(value, "value");
            try
            {
                var typecode = Type.GetTypeCode(typeof(T));
                if (typecode == TypeCode.Object)
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
                }
                else
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Cannot convert '{0}' to {1}.", value, typeof(T));
                throw new InvalidCastException(message, ex);
            }
        }

        /// <summary>
        /// 把字符串转成指定的类型，简单类型通过Convert.ChangeType进行转换，object类型通过json反序列化进行转换
        /// </summary>
        /// <param name="value">要进行转换的对象的字符串表现形式</param>
        /// <param name="type">目标类型</param>
        /// <exception cref="ArgumentNullException">value为空或者为空字符串或者type为空</exception>
        /// <exception cref="InvalidCastException">value无法转成目标类型</exception>
        /// <returns></returns>
        public static object ChangeType(string value, Type type)
        {
            Check.NotEmpty(value, "value");
            Check.NotNull(type, "type");

            try
            {
                var typecode = Type.GetTypeCode(type);
                if (typecode == TypeCode.Object)
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject(value, type);
                }
                else
                {
                    return Convert.ChangeType(value, type);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Cannot convert '{0}' to {1}.", value, type);
                throw new InvalidCastException(message, ex);
            }
        }
    }
}
