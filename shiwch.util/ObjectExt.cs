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
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;
using shiwch.util.FastReflection;
using System.Reflection;

namespace shiwch.util
{
    /// <summary>
    /// Object的扩展方法
    /// </summary>
    public static class ObjectExt
    {
        /// <summary>
        /// 将对象转成Json格式
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 将对象转成Xml格式
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <exception cref="ArgumentNullException">obj为空</exception>
        /// <returns></returns>
        public static string ToXml(this object obj)
        {
            Check.NotNull(obj, "obj");

            using (var stream = new MemoryStream())
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(stream, obj);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// 将对象转成Xml格式
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">要转换的对象</param>
        /// <returns></returns>
        public static string ToXml<T>(this T obj)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, obj);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// 将对象转成干净的Xml格式
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="rootName">根节点名称</param>
        /// <exception cref="ArgumentNullException">obj为空</exception>
        /// <exception cref="ArgumentException">rootName为空或者仅由空白字符组成</exception>
        /// <returns></returns>
        public static string ToCleanXml(this object obj, string rootName)
        {
            Check.NotNull(obj, "obj");

            return ToCleanXmlInternal(obj.GetType(), obj, rootName);
        }

        /// <summary>
        /// 将对象转成干净的Xml格式
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="rootName">根节点名称</param>
        /// <exception cref="ArgumentNullException">obj为空</exception>
        /// <exception cref="ArgumentException">rootName为空或者仅由空白字符组成</exception>
        /// <returns></returns>
        public static string ToCleanXml<T>(this T obj, string rootName)
        {
            return ToCleanXmlInternal(typeof(T), obj, rootName);
        }

        private static string ToCleanXmlInternal(Type type, object obj, string rootName)
        {
            Check.NotNull(obj, "obj");
            Check.NotEmpty(rootName, "rootName");

            using (var stream = new MemoryStream())
            {
                var serializer = new XmlSerializer(type);
                serializer.Serialize(stream, obj);
                XDocument doc = XDocument.Parse(Encoding.UTF8.GetString(stream.ToArray()));
                XElement ele = new XElement(rootName);
                foreach (var e in doc.Root.Elements())
                {
                    ele.Add(e);
                }
                return ele.ToString();
            }
        }

        /// <summary>
        /// 将json格式的字符串转成对象
        /// </summary>
        /// <param name="json">要转换的json字符串</param>
        /// <param name="type">目标对象类型</param>
        /// <exception cref="ArgumentNullException">type为空</exception>
        /// <returns></returns>
        public static object FromJson(string json, Type type)
        {
            Check.NotNull(type, "type");

            if (string.IsNullOrWhiteSpace(json)) return null;

            return Newtonsoft.Json.JsonConvert.DeserializeObject(json, type);
        }

        /// <summary>
        /// 将json格式的字符串转成对象
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="json">要转换的json字符串</param>
        /// <returns></returns>
        public static T FromJson<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return default(T);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 将xml格式的字符串转成对象
        /// </summary>
        /// <param name="xml">要转换的xml字符串</param>
        /// <param name="type">目标对象类型</param>
        /// <exception cref="ArgumentNullException">type为空</exception>
        /// <returns></returns>
        public static object FromXml(string xml, Type type)
        {
            Check.NotNull(type, "type");

            if (string.IsNullOrWhiteSpace(xml)) return null;

            var serializer = new XmlSerializer(type);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                return serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// 将xml格式的字符串转成对象
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="xml">要转换的xml字符串</param>
        /// <returns></returns>
        public static T FromXml<T>(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) return default(T);

            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                return (T)Convert.ChangeType(serializer.Deserialize(stream), typeof(T));
            }
        }
    }
}
