using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shiwch.game
{
    public static class ParamHelper
    {
        /// <summary>
        /// Redis连接串
        /// </summary>
        public static string Redis { get; set; }
        /// <summary>
        /// 数据库连接串
        /// </summary>
        public static string DataDb { get; set; }

        public static async Task<IDbConnection> OpenDbAsync(string dbconn = null)
        {
            if (dbconn == null) dbconn = DataDb;
            SqlConnection conn = new SqlConnection(dbconn);
            await conn.OpenAsync();
            return conn;
        }
        public static IDbConnection OpenDb(string dbconn = null)
        {
            if (dbconn == null) dbconn = DataDb;
            SqlConnection conn = new SqlConnection(dbconn);
            conn.Open();
            return conn;
        }
    }
}
