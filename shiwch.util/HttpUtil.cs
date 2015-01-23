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
using System.Net;
using log4net;
using System.IO;
using System.Collections.Specialized;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;

namespace shiwch.util
{
    /// <summary>
    /// Http助手类
    /// </summary>
    public static class HttpUtil
    {
        private static readonly ILog logger = LogManager.GetLogger("HttpTrace");
        /// <summary>
        /// 获取指定Url的内容
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="encoding">用于对字符串进行编码的 Encoding，默认使用Encoding.Default</param>
        /// <exception cref="ArgumentNullException">url为空</exception>
        /// <returns></returns>
        public static async Task<string> GetHttpUrl(string url, Encoding encoding = null)
        {
            Check.NotEmpty(url, "url");

            if (encoding == null) encoding = Encoding.Default;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    var result = encoding.GetString(bytes);
                    if (logger.IsDebugEnabled) logger.DebugFormat("Get[{0}], Response[{1}]", url, result);
                    return result;
                }
                catch (Exception ex)
                {
                    logger.Error(string.Format("Get[{0}]异常", url), ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Post数据，并使用指定的Encoding解码返回的数据
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="data">数据</param>
        /// <param name="encoding">Encoding，默认使用Encoding.Default</param>
        /// <exception cref="ArgumentNullException">url或者data为空</exception>
        /// <returns></returns>
        public static async Task<string> PostData(string url, IEnumerable<KeyValuePair<string, string>> data, Encoding encoding = null)
        {
            Check.NotEmpty(url, "url");
            Check.NotNull(data, "data");

            if (encoding == null) encoding = Encoding.Default;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(data));
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    var result = encoding.GetString(bytes);
                    if (logger.IsDebugEnabled) logger.DebugFormat("Get[{0}], Response[{1}]", url, result);
                    return result;
                }
                catch (Exception ex)
                {
                    logger.Error(string.Format("Post[{0}], Data[{1}]异常", url, ToQueryString(data)), ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// 转成url格式
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static string ToQueryString(IEnumerable<KeyValuePair<string, string>> data)
        {
            if (data == null) return "";

            string value = string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var pair in data)
            {
                stringBuilder.Append(value);
                stringBuilder.Append(HttpUtility.UrlEncode(pair.Key));
                stringBuilder.Append("=");
                stringBuilder.Append(HttpUtility.UrlEncode(pair.Value));
                value = "&";
            }
            return stringBuilder.ToString();
        }

    }
}
