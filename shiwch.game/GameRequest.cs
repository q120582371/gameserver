using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shiwch.game
{
    public class GameRequest
    {
        private IDictionary<string, string> m_params;

        public GameRequest(IDictionary<string, string> param)
        {
            this.m_params = param;
        }

        public IDictionary<string, string> Params { get { return m_params; } }

        public string this[string key]
        {
            get
            {
                if (!m_params.ContainsKey(key)) return null;
                return m_params[key];
            }
            set
            {
                m_params[key] = value;
            }
        }

        public bool Contains(string key)
        {
            return m_params.ContainsKey(key);
        }

        /// <summary>
        /// 请求者Ip
        /// </summary>
        public string UserHostAddress { get { return this["UserHostAddress"]; } }
        /// <summary>
        /// 是否已登录
        /// </summary>
        public bool IsAuthenticated { get; internal set; }
        /// <summary>
        /// ActionId
        /// </summary>
        public int ActionId { get { return Convert.ToInt32(this["ActionId"]); } }
        public int MsgId { get { return Convert.ToInt32(this["MsgId"]); } }
    }
}
