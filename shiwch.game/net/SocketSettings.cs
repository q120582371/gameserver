using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace shiwch.game.net
{
    public class SocketSettings
    {
        public SocketSettings(int maxConnections, int backlog, int recvSize, int sendSize, IPEndPoint localEndPoint, int heartbeat)
        {
            this.MaxConnections = maxConnections;
            this.Backlog = backlog;
            this.RecvSize = recvSize;
            this.SendSize = sendSize;
            this.LocalEndPoint = localEndPoint;
            this.Heartbeat = heartbeat;
        }

        public int MaxConnections { get; private set; }
        public int Backlog { get; private set; }
        public int RecvSize { get; private set; }
        public int SendSize { get; private set; }
        public IPEndPoint LocalEndPoint { get; private set; }
        public int Heartbeat { get; private set; }
    }
}
