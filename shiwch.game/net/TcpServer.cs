using shiwch.util.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace shiwch.game.net
{
    public class TcpConnection
    {
        public Socket Socket { get; internal set; }
        public byte[] SessionId { get; internal set; }
        public DateTime ActiveTime { get; internal set; }
        internal DateTime LastHeartbeatTime { get; set; }
        public bool IsBackground { get; internal set; }
        public EndPoint RemoteEndPoint { get; internal set; }
        public object UserToken { get; set; }
    }
    class TempConnection
    {
        public Socket Socket { get; set; }
        public byte[] Token { get; set; }
    }
    public class TcpServer
    {
        SocketListener listener;
        SocketSettings socketSettings;
        byte[] md5key = new byte[] { 0x07, 0x0A, 0x05, 0xFF, 0xDF, 0x8B, 0x6C, 0x3A };
        Timer heartbeatTimer;
        int recvnum, sendnum;
        int recvBytes, sendBytes;
        long totalrecvnum, totalsendnum;
        long totalrecvBytes, totalsendBytes;
        ConcurrentDictionary<Socket, TcpConnection> allConnectionDict = new ConcurrentDictionary<Socket, TcpConnection>();
        ConcurrentDictionary<Socket, TempConnection> allTempConnectionDict = new ConcurrentDictionary<Socket, TempConnection>();
        static readonly ILog logger = LogManager.GetLogger("socket");
        int idcounter;
        DateTime lastStatTime;

        public IEnumerable<TcpConnection> AllSessions
        {
            get
            {
                return allConnectionDict.Values;
            }
        }

        public Action<TcpConnection, byte[]> OnData { get; set; }
        public Action<TcpConnection> OnConnect { get; set; }
        public Action<TcpConnection, SocketError, string> OnClose { get; set; }
        public Action<TcpConnection, Exception> OnError { get; set; }

        public TcpServer(SocketSettings socketSettings)
        {
            listener = new SocketListener(socketSettings);
            this.socketSettings = socketSettings;
            heartbeatTimer = new Timer(Heartbeat, null, 1000, Timeout.Infinite);
            lastStatTime = DateTime.Now;
            listener.OnOpen = onOpen;
            listener.OnData = onData;
            listener.OnClose = onClose;
            listener.OnError = onError;
        }

        private void onOpen(Socket socket)
        {
            var conn = new TempConnection { Socket = socket };
            allTempConnectionDict[socket] = conn;
            SendHandshake(conn);
            Task.Delay(30000).ContinueWith((t) =>
            {
                if (!allConnectionDict.ContainsKey(socket))
                {
                    listener.CloseSocket(socket, "invalid connection");
                }
            });
        }
        private void onData(Socket socket, byte head, byte[] data)
        {
            Interlocked.Increment(ref recvnum);
            Interlocked.Add(ref recvBytes, data.Length + 4);
            if ((head & 0x1F) == 0x1)
            {//握手
                TempConnection connection;
                if (!allTempConnectionDict.TryGetValue(socket, out connection))
                {
                    listener.CloseSocket(socket, "invalid handshake");
                }
                else
                {
                    //4字节unixtime
                    var source = Util.Concat(connection.Token, 0, connection.Token.Length, md5key);
                    var mysign = Util.MD5(source, 0, source.Length);
                    if (!Util.ByteEquals(data, 0, 16, mysign))
                    {//签名错误
                        listener.CloseSocket(socket, "invalid sign");
                    }
                    else
                    {
                        allTempConnectionDict.TryRemove(socket, out connection);
                        byte[] sessionId = new byte[6];
                        if (data.Length == 26)
                        {//客户端上传了sessionid
                            sessionId[0] = data[16];
                            sessionId[1] = data[17];
                            sessionId[2] = data[18];
                            sessionId[3] = data[19];
                            sessionId[4] = data[20];
                            sessionId[5] = data[21];
                        }
                        else
                        {//新生成一个sessionid
                            var tmp = BitConverter.GetBytes(Util.ToUnixTime(DateTime.Now));
                            sessionId[0] = tmp[0];
                            sessionId[1] = tmp[1];
                            sessionId[2] = tmp[2];
                            sessionId[3] = tmp[3];
                            tmp = BitConverter.GetBytes((UInt16)Interlocked.Increment(ref idcounter));
                            sessionId[4] = tmp[0];
                            sessionId[5] = tmp[1];
                        }
                        var session = new TcpConnection { Socket = socket, ActiveTime = DateTime.Now, SessionId = sessionId, RemoteEndPoint = socket.RemoteEndPoint, LastHeartbeatTime = DateTime.Now };
                        allConnectionDict[socket] = session;
                        SendSessionId(session);
                        if (OnConnect != null) OnConnect(session);
                    }
                }
            }
            else if ((head & 0x1F) == 0x4)
            {//客户端对心跳的回应
                logger.DebugFormat("receive heartbeat response from {0}", socket.RemoteEndPoint);
                bool isBackground = (head & 0x80) == 0x80;
                TcpConnection session;
                if (allConnectionDict.TryGetValue(socket, out session))
                {
                    session.ActiveTime = DateTime.Now;
                    session.IsBackground = isBackground;
                }
            }
            else if ((head & 0x1F) == 0x3)
            {//客户端主动发来心跳
                logger.DebugFormat("receive heartbeat from {0}", socket.RemoteEndPoint);
                SendHeartbeatResponse(socket);
                bool isBackground = (head & 0x80) == 0x80;
                TcpConnection session;
                if (allConnectionDict.TryGetValue(socket, out session))
                {
                    session.ActiveTime = DateTime.Now;
                    session.IsBackground = isBackground;
                }
            }
            else
            {//数据包
                TcpConnection session;
                if (allConnectionDict.TryGetValue(socket, out session))
                {
                    session.ActiveTime = DateTime.Now;
                    if (OnData != null) OnData(session, data);
                }
                else
                {
                    listener.CloseSocket(socket, "invalid data");
                }
            }
        }
        private void onClose(Socket socket, SocketError error, string reason)
        {
            TempConnection tmp1;
            allTempConnectionDict.TryRemove(socket, out tmp1);
            TcpConnection session;
            if (allConnectionDict.TryRemove(socket, out session))
            {
                if (OnClose != null) OnClose(session, error, reason);
            }
        }
        private void onError(Socket socket, Exception ex)
        {
            TcpConnection session;
            if (allConnectionDict.TryGetValue(socket, out session))
            {
                if (OnError != null) OnError(session, ex);
            }
        }
        private void Heartbeat(object state)
        {
            heartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
            try
            {
                foreach (var conn in allConnectionDict)
                {
                    var ts = DateTime.Now.Subtract(conn.Value.ActiveTime).TotalSeconds;
                    if (ts >= 3 * socketSettings.Heartbeat)
                    {
                        listener.CloseSocket(conn.Value.Socket, "Heartbeat Timeout");
                    }
                    else if (ts >= socketSettings.Heartbeat)
                    {
                        ts = DateTime.Now.Subtract(conn.Value.LastHeartbeatTime).TotalSeconds;
                        if (ts >= socketSettings.Heartbeat)
                        {
                            conn.Value.LastHeartbeatTime = DateTime.Now;
                            SendHeartbeat(conn.Value.Socket);
                            if (logger.IsDebugEnabled) logger.DebugFormat("Send heartbeat to {0}", conn.Value.RemoteEndPoint);
                        }
                    }
                }
                var span = DateTime.Now.Subtract(lastStatTime).TotalSeconds;
                if (span >= 10)
                {
                    lastStatTime = DateTime.Now;
                    var cur_recvBytes = Interlocked.Exchange(ref recvBytes, 0);
                    var cur_sendBytes = Interlocked.Exchange(ref sendBytes, 0);
                    var cur_recvnum = Interlocked.Exchange(ref recvnum, 0);
                    var cur_sendnum = Interlocked.Exchange(ref sendnum, 0);
                    logger.WarnFormat("socket信息\r\n当前连接数：{0}\r\n总发送包数：{1}\r\n总接收包数：{2}\r\n总发送字节：{3}\r\n总接收字节：{4}\r\n最近发送包数：{5}\r\n最近接收包数：{6}\r\n最近发送字节数：{7}\r\n最近接收字节数：{8}\r\n最近包发送速率：{9}/s\r\n最近包接收速率：{10}/s\r\n最近字节发送速率：{11}/s\r\n最近字节接收速率：{12}/s\r\n总速率：{14}\r\n发送池：{13}\r\n接收池：{15}", allConnectionDict.Count, totalsendnum, totalrecvnum, Short(totalsendBytes), Short(totalrecvBytes), cur_sendnum, cur_recvnum, Short(cur_sendBytes), Short(cur_recvBytes), (cur_sendnum / span).ToString("0.00"), (cur_recvnum / span).ToString("0.00"), Short(cur_sendBytes / span), Short(cur_recvBytes / span), listener.SendArgsPoolCount, Short(((long)cur_recvBytes + (long)cur_sendBytes) * 8 / span).Replace("B", "bps"), listener.RecvArgsPoolCount);
                    totalrecvBytes += cur_recvBytes;
                    totalsendBytes += cur_sendBytes;
                    totalrecvnum += cur_recvnum;
                    totalsendnum += cur_sendnum;
                }
            }
            finally
            {
                heartbeatTimer.Change(1000, Timeout.Infinite);
            }
        }
        private static string Short(double v)
        {
            if (v > 1024 * 1024 * 1024) return (v / 1024 / 1024 / 1024).ToString("0.00") + "GB";
            if (v > 1024 * 1024) return (v / 1024 / 1024).ToString("0.00") + "MB";
            if (v > 1024) return (v / 1024).ToString("0.00") + "KB";
            return v.ToString("0.00") + "B";
        }

        private static byte head_Data = 0x0;
        private static byte head_Handshake = 0x1;
        private static byte head_HandshakeSuccess = 0x2;
        private static byte head_Heartbeat = 0x3;
        private static byte head_HeartbeatResponse = 0x4;
        private static byte[] empty = new byte[1];

        private void SendHeartbeatResponse(Socket socket)
        {
            Interlocked.Increment(ref sendnum);
            Interlocked.Add(ref sendBytes, 4);

            try
            {
                listener.SendData(socket, empty, head_HeartbeatResponse);
            }
            catch { }
        }
        private void SendHeartbeat(Socket socket)
        {
            Interlocked.Increment(ref sendnum);
            Interlocked.Add(ref sendBytes, 4);

            try
            {
                listener.SendData(socket, empty, head_Heartbeat);
            }
            catch { }
        }
        private void SendHandshake(TempConnection conn)
        {
            Interlocked.Increment(ref sendnum);
            Interlocked.Add(ref sendBytes, 8);

            conn.Token = BitConverter.GetBytes(Util.ToUnixTime(DateTime.Now));
            try
            {
                listener.SendData(conn.Socket, conn.Token, head_Handshake);
            }
            catch { }
        }
        private void SendSessionId(TcpConnection session)
        {
            Interlocked.Increment(ref sendnum);
            Interlocked.Add(ref sendBytes, session.SessionId.Length + 4);
            try
            {
                listener.SendData(session.Socket, session.SessionId, head_HandshakeSuccess);
            }
            catch { }
        }

        public void Start()
        {
            listener.StartListen();
        }

        public void SendData(TcpConnection session, byte[] data)
        {
            listener.SendData(session.Socket, data, head_Data);
        }

        public void SendData(TcpConnection session, byte[] data, int offset, int count)
        {
            Interlocked.Increment(ref sendnum);
            Interlocked.Add(ref sendBytes, count + 4);

            listener.SendData(session.Socket, data, offset, count, head_Data);
        }

        public void PostMessage(Func<object, Task> action, object state)
        {
            listener.PostMessage(action, state);
        }

        public void CloseSession(TcpConnection session, string errMsg)
        {
            listener.CloseSocket(session.Socket, errMsg);
        }
    }
}
