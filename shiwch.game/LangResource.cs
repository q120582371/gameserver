using shiwch.util;
using shiwch.util.Dapper;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace shiwch.game
{
    public static class LangResource
    {
        private static ConcurrentDictionary<string, string> resource = new ConcurrentDictionary<string, string>();
        private static Timer timer;
        public static void Init()
        {
            LoadData(null);
            timer = new Timer(LoadData, null, 300000, 300000);
        }
        private static void LoadData(object state)
        {
            using (var conn = ParamHelper.OpenDb())
            {
                var sql = "select * from LangResource";
                foreach (var row in conn.Query(sql))
                {
                    resource[(string)row.ResourceId] = (string)row.Resource;
                }
            }
        }
        /// <summary>
        /// 获取指定资源Id对应的资源，并格式化
        /// </summary>
        public static string GetResource(string resourceId, params object[] args)
        {
            if (resource.ContainsKey(resourceId))
            {
                if (args.Length == 0) return resource[resourceId];
                else return string.Format(resource[resourceId], args);
            }
            return resourceId;
        }

        /// <summary>
        /// 获取指定资源Id对应的资源，并格式化
        /// </summary>
        public static string GetResource(string resourceId, IDictionary<string, object> args)
        {
            if (resource.ContainsKey(resourceId))
            {
                if (args.Count == 0) return resource[resourceId];
                else return StringExt.FormatX(resource[resourceId], args);
            }
            return resourceId;
        }
    }
}
