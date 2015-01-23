using shiwch.util.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shiwch.util.Dapper;

namespace shiwch.util
{
    public static class Util
    {
        /// <summary>
        /// 重试直到返回值不为空
        /// </summary>
        public static object DoWhileNull(Func<object> func)
        {
            do
            {
                object result = func();
                if (result != null) return result;
            } while (true);
        }

        /// <summary>
        /// 重试直到无异常
        /// </summary>
        public static object DoWhileException(Func<object> func)
        {
            do
            {
                try
                {
                    return func();
                }
                catch (Exception ex)
                {
                    LogManager.Error("Util", ex);
                }
            } while (true);
        }

        /// <summary>
        /// 堆栈跟踪输出
        /// </summary>
        public static string StackTrace()
        {
            StackTrace st = new StackTrace(true);
            string stackIndent = "";
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < st.FrameCount; i++)
            {
                StackFrame sf = st.GetFrame(i);
                //得到错误的方法
                var method = sf.GetMethod();
                builder.AppendFormat(stackIndent + "Method: {0}\r\n", method);
                //得到错误的文件名
                builder.AppendFormat(stackIndent + "File: {0}\r\n", sf.GetFileName());
                //得到文件错误的行号
                builder.AppendFormat(stackIndent + "Line: {0}\r\n", sf.GetFileLineNumber());
                stackIndent += "++";
            }

            return builder.ToString();
        }

        /// <summary>
        /// 加载数据库表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn">数据库连接</param>
        /// <param name="filter">where语句，例如：a=5 and b=6</param>
        /// <param name="orderby">order by 字句，例如：a,b desc</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static List<T> LoadTable<T>(this IDbConnection conn, string filter = "", string orderby = "", dynamic param = null)
        {
            var sql = "select * from [{0}] where 1=1";
            if (!string.IsNullOrEmpty(filter)) sql += " and " + filter;
            if (!string.IsNullOrEmpty(orderby)) sql += " order by " + orderby;
            return conn.Query<T>(sql, (object)param).ToList();
        }
        /// <summary>
        /// 异步加载数据库表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn">数据库连接</param>
        /// <param name="filter">where语句，例如：a=5 and b=6</param>
        /// <param name="orderby">order by 字句，例如：a,b desc</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static async Task<List<T>> LoadTableAsync<T>(this IDbConnection conn, string filter = "", string orderby = "", dynamic param = null)
        {
            var sql = "select * from [{0}] where 1=1";
            if (!string.IsNullOrEmpty(filter)) sql += " and " + filter;
            if (!string.IsNullOrEmpty(orderby)) sql += " order by " + orderby;
            var result = await conn.QueryAsync<T>(sql, (object)param);
            return result.ToList();
        }
    }
}
