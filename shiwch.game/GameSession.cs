using shiwch.game.net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shiwch.game
{
    public class GameSession
    {
        private object userObject;
        private TcpConnection tcpConnection;
        private Dictionary<string, object> sessionSlot = new Dictionary<string, object>();
        private Dictionary<string, object> tempSlot = new Dictionary<string, object>();

        public object User
        {
            get { return userObject; }
        }

        /// <summary>
        /// session临时存储，整个会话期间存活
        /// </summary>
        public IDictionary<string, object> TempSlot { get { return tempSlot; } }
        /// <summary>
        /// session临时存储，每次请求结束后清除
        /// </summary>
        public IDictionary<string, object> SessionSlot { get { return sessionSlot; } }

        public void Send(object message)
        {

        }
    }
}
