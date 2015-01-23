using shiwch.game.net;
using shiwch.util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace shiwch.game
{
    public class GameApplication
    {
        struct SessionId : IEquatable<SessionId>
        {
            private byte[] sessionid;

            public SessionId(byte[] sessionid)
            {
                this.sessionid = sessionid;
            }

            public bool Equals(SessionId other)
            {
                return ByteArrayEqualityComparer.ByteEquals(sessionid, other.sessionid);
            }
            public override int GetHashCode()
            {
                int hashcode = sessionid[0].GetHashCode();
                for (int i = 1; i < sessionid.Length; i++)
                {
                    hashcode = hashcode ^ sessionid[i].GetHashCode();
                }
                return hashcode;
            }
        }

        private TcpServer tcpServer;
        private IRequestParamParser paramParser;
        private IResponseFormater responseFormater;
        private ConcurrentDictionary<int, HandlerInfo> handlers = new ConcurrentDictionary<int, HandlerInfo>();
        private ConcurrentDictionary<TcpConnection, GameSession> sessions = new ConcurrentDictionary<TcpConnection, GameSession>();
        private ConcurrentDictionary<long, GameSession> user_session_map = new ConcurrentDictionary<long, GameSession>();

        public void Init()
        {
            var localEndPoint = new IPEndPoint(IPAddress.Any, 9527);
            SocketSettings socketSettings = new SocketSettings(20000, 10, 512, 1024, localEndPoint, 60);
            tcpServer = new TcpServer(socketSettings);
            tcpServer.OnConnect = OnConnect;
            tcpServer.OnError = OnError;
            tcpServer.OnClose = OnClose;
            tcpServer.OnData = OnData;
            tcpServer.Start();
        }

        private void OnData(TcpConnection tcpConnection, byte[] requestBody)
        {
            var task = Process(tcpConnection, requestBody);
            task.ContinueWith(t =>
            {
                byte[] data = responseFormater.Formate(t.Result);
                tcpServer.SendData(tcpConnection, data);
            });
        }

        private void OnConnect(TcpConnection tcpConnection)
        {

        }
        private void OnError(TcpConnection tcpConnection, Exception exception)
        {

        }
        private void OnClose(TcpConnection tcpConnection, SocketError socketError, string error)
        {

        }

        private async Task<GameResponse> Process(TcpConnection tcpConnection, byte[] requestBody)
        {
            GameSession session = null;
            if (!sessions.TryGetValue(tcpConnection, out session))
            {
                session = new GameSession();
                sessions[tcpConnection] = session;
            }
            var param = paramParser.Parse(requestBody);
            GameRequest request = new GameRequest(param);
            GameResponse response = new GameResponse();
            GameContext context = new GameContext(request, response, session);

            response.ActionId = request.ActionId;
            response.ReplyTo = request.MsgId;

            HandlerInfo handlerInfo = null;
            if (!handlers.TryGetValue(request.ActionId, out handlerInfo))
            {
                response.StatusCode = StatusCode.ParamError;
                response.Description = string.Format("ActionId={0}未实现", request.ActionId);
                return response;
            }

            GameHandler handler = (GameHandler)FastActivator.Create(handlers[request.ActionId].Type);
            if (!handler.Init(param)) return response;

            await handler.DoWork();

            return response;
        }
    }
}
