using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shiwch.game
{
    public class GameContext
    {
        private GameRequest request;
        private GameResponse response;
        private GameSession session;
        private GameHandler handler;

        /// <summary>
        /// 请求对象
        /// </summary>
        public GameRequest Request { get { return request; } }
        /// <summary>
        /// 响应对象
        /// </summary>
        public GameResponse Response { get { return response; } }
        /// <summary>
        /// Session对象
        /// </summary>
        public GameSession Session { get { return session; } }
        /// <summary>
        /// 处理器对象
        /// </summary>
        public GameHandler Handler { get { return handler; } internal set { handler = value; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="session"></param>
        public GameContext(GameRequest request, GameResponse response, GameSession session)
        {
            this.request = request;
            this.response = response;
            this.session = session;
        }
    }
}
