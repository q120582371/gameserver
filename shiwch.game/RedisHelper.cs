using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shiwch.game
{
    public static class RedisHelper
    {
        private static object syncRoot = new object();
        private static ConcurrentDictionary<string, ConnectionMultiplexer> dict = new ConcurrentDictionary<string, ConnectionMultiplexer>();

        public static ConnectionMultiplexer GetMultiplexer(string conn)
        {
            ConnectionMultiplexer result;
            if (!dict.TryGetValue(conn, out result))
            {
                lock (syncRoot)
                {
                    if (!dict.TryGetValue(conn, out result))
                    {
                        ConfigurationOptions options = new ConfigurationOptions();
                        options.AbortOnConnectFail = false;
                        options.AllowAdmin = true;
                        options.EndPoints.Add(conn);
                        result = ConnectionMultiplexer.Connect(options);
                        dict[conn] = result;
                    }
                }
            }
            return result;
        }

        public static ConnectionMultiplexer GetMultiplexer(string host, int port)
        {
            var conn = host + ":" + port;
            return GetMultiplexer(conn);
        }
    }
}
