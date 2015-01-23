using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text;
using System.Runtime.InteropServices;
using shiwch.util.Logging;

namespace shiwch.game.net
{
    class CallbackArgs
    {
        public CallbackArgs(SendOrPostCallback callback, object state)
        {
            this.Callback = callback;
            this.State = state;
        }
        public SendOrPostCallback Callback { get; private set; }
        public object State { get; private set; }
    }
    class ActionArgs
    {
        public ActionArgs(Func<object, Task> action, object state)
        {
            this.Action = action;
            this.State = state;
        }
        public Func<object, Task> Action { get; private set; }
        public object State { get; private set; }
    }
    class MessageArgs
    {
        public MessageArgs(Socket socket, byte head, byte[] data)
        {
            if (socket == null) throw new ArgumentNullException("socket");
            this.Socket = socket;
            this.Data = data;
            this.Head = head;
        }
        public Socket Socket { get; private set; }
        public byte[] Data { get; private set; }
        public byte Head { get; private set; }
    }
    class CloseArgs
    {
        public CloseArgs(Socket socket, SocketError socketError, string reason)
        {
            if (socket == null) throw new ArgumentNullException("socket");
            this.Socket = socket;
            this.SocketError = socketError;
            this.Reason = reason;
        }
        public Socket Socket { get; private set; }
        public SocketError SocketError { get; private set; }
        public string Reason { get; private set; }
    }
    class ErrorArgs
    {
        public ErrorArgs(Socket socket, Exception exception, SocketError socketError)
        {
            if (socket == null) throw new ArgumentNullException("socket");
            this.Socket = socket;
            this.Exception = exception;
            this.SocketError = socketError;
        }
        public Socket Socket { get; private set; }
        public Exception Exception { get; private set; }
        public SocketError SocketError { get; private set; }
    }

    class SocketListener
    {
        BufferManager recvBufferManager;
        BufferManager sendBufferManager;
        Socket listenSocket;
        Semaphore maxConnectionsEnforcer;
        SocketSettings socketSettings;
        PrefixHandler prefixHandler;
        MessageHandler messageHandler;
        ThreadSafeStack<SocketAsyncEventArgs> acceptArgsPool;
        ThreadSafeStack<SocketAsyncEventArgs> recvArgsPool;
        ThreadSafeStack<SocketAsyncEventArgs> sendArgsPool;
        ConcurrentDictionary<Socket, ExSocket> exSocketDict = new ConcurrentDictionary<Socket, ExSocket>();
        static readonly ILog logger = LogManager.GetLogger("socket");

        public int RecvArgsPoolCount { get { return recvArgsPool.Count; } }
        public int SendArgsPoolCount { get { return sendArgsPool.Count; } }

        byte[] md5key = new byte[] { 0x7, 0xA, 0x5, 0xFF, 0xDF, 0x8B, 0x6C, 0x3A };

        public Action<Socket, byte, byte[]> OnData { get; set; }
        public Action<Socket> OnOpen { get; set; }
        public Action<Socket, SocketError, string> OnClose { get; set; }
        public Action<Socket, Exception> OnError { get; set; }

        BlockingCollection<object> messagequeue = new BlockingCollection<object>(new ConcurrentQueue<object>());
        Thread messageThread;

        public SocketListener(SocketSettings socketSettings)
        {
            this.socketSettings = socketSettings;
            this.prefixHandler = new PrefixHandler();
            this.messageHandler = new MessageHandler();

            this.recvBufferManager = new BufferManager(this.socketSettings.RecvSize * this.socketSettings.MaxConnections, this.socketSettings.RecvSize);
            this.sendBufferManager = new BufferManager(this.socketSettings.SendSize * this.socketSettings.MaxConnections, this.socketSettings.SendSize);

            this.recvArgsPool = new ThreadSafeStack<SocketAsyncEventArgs>(socketSettings.MaxConnections);
            this.sendArgsPool = new ThreadSafeStack<SocketAsyncEventArgs>(socketSettings.MaxConnections);
            this.acceptArgsPool = new ThreadSafeStack<SocketAsyncEventArgs>(100);
            this.maxConnectionsEnforcer = new Semaphore(this.socketSettings.MaxConnections, this.socketSettings.MaxConnections);

            Init();

            #region messageThread
            messageThread = new Thread(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new ThreadSynchronizationContext(messagequeue));
                foreach (var args in messagequeue.GetConsumingEnumerable())
                {
                    try
                    {
                        if (args is MessageArgs) ProcessMessage((MessageArgs)args);
                        else if (args is CloseArgs) ProcessClose((CloseArgs)args);
                        else if (args is Socket) ProcessOpen((Socket)args);
                        else if (args is ErrorArgs) ProcessError((ErrorArgs)args);
                        else if (args is CallbackArgs) ProcessCallback((CallbackArgs)args);
                        else if (args is ActionArgs) ProcessAction((ActionArgs)args);
                        else throw new NotSupportedException(args.ToString());
                    }
                    catch (Exception ex)
                    {
                        logger.Error("未知异常", ex);
                    }
                }
            });
            messageThread.IsBackground = true;
            messageThread.Name = "message";
            messageThread.Start();
            #endregion
        }

        private void ProcessMessage(MessageArgs args)
        {
            if (OnData != null)
            {
                OnData(args.Socket, args.Head, args.Data);
            }
        }
        private void ProcessOpen(Socket socket)
        {
            if (OnOpen != null)
            {
                OnOpen(socket);
            }
        }
        private void ProcessClose(CloseArgs args)
        {
            if (OnClose != null)
            {
                OnClose(args.Socket, args.SocketError, args.Reason);
            }
        }
        private void ProcessError(ErrorArgs args)
        {
            if (OnError != null)
            {
                OnError(args.Socket, args.Exception);
            }
        }
        private void ProcessCallback(CallbackArgs args)
        {
            args.Callback(args.State);
        }
        private void ProcessAction(ActionArgs args)
        {
            args.Action(args.State);
        }

        private void Init()
        {
            this.recvBufferManager.InitBuffer();
            this.sendBufferManager.InitBuffer();

            for (int i = 0; i < 10; i++)
            {
                this.acceptArgsPool.Push(CreateAcceptEventArgs());
            }

            SocketAsyncEventArgs recvArgs, sendArgs;
            DataToken dataToken;
            for (int i = 0; i < this.socketSettings.MaxConnections; i++)
            {
                recvArgs = new SocketAsyncEventArgs();
                recvBufferManager.SetBuffer(recvArgs);
                recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                dataToken = new DataToken();
                dataToken.bufferOffset = recvArgs.Offset;
                recvArgs.UserToken = dataToken;
                recvArgsPool.Push(recvArgs);

                sendArgs = new SocketAsyncEventArgs();
                sendBufferManager.SetBuffer(sendArgs);
                sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                dataToken = new DataToken();
                dataToken.bufferOffset = sendArgs.Offset;
                sendArgs.UserToken = dataToken;
                sendArgsPool.Push(sendArgs);
            }
        }

        private SocketAsyncEventArgs CreateAcceptEventArgs()
        {
            SocketAsyncEventArgs acceptEventArg = new SocketAsyncEventArgs();
            acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed);
            return acceptEventArg;
        }

        public void StartListen()
        {
            listenSocket = new Socket(this.socketSettings.LocalEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            listenSocket.Bind(this.socketSettings.LocalEndPoint);
            listenSocket.Listen(socketSettings.Backlog);
            PostAccept();
        }

        private void PostAccept()
        {
            SocketAsyncEventArgs acceptEventArgs;
            if (!acceptArgsPool.TryPop(out acceptEventArgs))
                acceptEventArgs = CreateAcceptEventArgs();
            this.maxConnectionsEnforcer.WaitOne();

            bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArgs);
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArgs);
            }
        }

        private void Accept_Completed(object sender, SocketAsyncEventArgs acceptEventArgs)
        {
            ProcessAccept(acceptEventArgs);
        }

        private void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            PostAccept();

            if (acceptEventArgs.SocketError != SocketError.Success)
            {
                try
                {
                    acceptEventArgs.AcceptSocket.Close();
                }
                catch { }
                acceptEventArgs.AcceptSocket = null;
                acceptArgsPool.Push(acceptEventArgs);
                maxConnectionsEnforcer.Release();
                return;
            }
            exSocketDict[acceptEventArgs.AcceptSocket] = new ExSocket();
            messagequeue.Add(acceptEventArgs.AcceptSocket);

            logger.DebugFormat("accepting connection {0}", acceptEventArgs.AcceptSocket.RemoteEndPoint);

            SocketAsyncEventArgs recvArgs;
            if (!recvArgsPool.TryPop(out recvArgs)) throw new InvalidOperationException("没有足够的SocketAsyncEventArgs供分配");

            recvArgs.AcceptSocket = acceptEventArgs.AcceptSocket;
            acceptEventArgs.AcceptSocket = null;
            acceptArgsPool.Push(acceptEventArgs);
            var dataToken = (DataToken)recvArgs.UserToken;
            recvArgs.SetBuffer(dataToken.bufferOffset, socketSettings.RecvSize);

            PostReceive(recvArgs);
        }

        private void IO_Completed(object sender, SocketAsyncEventArgs ioEventArgs)
        {
            try
            {
                DataToken ioDataToken = (DataToken)ioEventArgs.UserToken;
                switch (ioEventArgs.LastOperation)
                {
                    case SocketAsyncOperation.Receive:
                        ProcessReceive(ioEventArgs);
                        break;
                    case SocketAsyncOperation.Send:
                        ProcessSend(ioEventArgs);
                        break;
                    default:
                        throw new ArgumentException("The last operation completed on the socket was not a receive or send");
                }
            }
            catch (Exception ex)
            {
                logger.Error("IO_Completed", ex);
            }
        }
        private void ReleaseRecvArgs(SocketAsyncEventArgs recvArgs)
        {
            var dataToken = (DataToken)recvArgs.UserToken;
            dataToken.Reset(true);
            recvArgs.AcceptSocket = null;
            recvArgsPool.Push(recvArgs);
        }
        private void ReleaseSendArgs(SocketAsyncEventArgs sendArgs)
        {
            var dataToken = (DataToken)sendArgs.UserToken;
            dataToken.Reset(true);
            sendArgs.AcceptSocket = null;
            sendArgsPool.Push(sendArgs);
        }
        /// <summary>
        /// 投递接收数据请求
        /// </summary>
        /// <param name="recvArgs"></param>
        private void PostReceive(SocketAsyncEventArgs recvArgs)
        {
            try
            {
                bool willRaiseEvent = recvArgs.AcceptSocket.ReceiveAsync(recvArgs);
                if (!willRaiseEvent)
                {
                    ProcessReceive(recvArgs);
                }
            }
            catch (ObjectDisposedException)
            {
                messagequeue.Add(new ErrorArgs(recvArgs.AcceptSocket, new Exception("connection drop(PostReceive)"), SocketError.HostDown));
                ReleaseRecvArgs(recvArgs);
            }
        }

        /// <summary>
        /// 处理数据接收回调
        /// </summary>
        private void ProcessReceive(SocketAsyncEventArgs recvArgs)
        {
            DataToken dataToken = (DataToken)recvArgs.UserToken;
            if (recvArgs.SocketError != SocketError.Success)
            {
                //Socket错误
                HandleCloseSocket(recvArgs, "receive error", true);
                return;
            }

            if (recvArgs.BytesTransferred == 0)
            {
                //对方主动关闭socket
                HandleCloseSocket(recvArgs, "connection closed by remote host", true);
                return;
            }

            bool needPostAnother = ParseData(recvArgs, dataToken);

            if (needPostAnother)
            {
                if (dataToken.prefixBytesDone == 4 && dataToken.IsMessageReady)
                {
                    dataToken.Reset(true);
                }
                dataToken.bufferSkip = 0;
                PostReceive(recvArgs);
            }
        }

        private bool ParseData(SocketAsyncEventArgs redvArgs, DataToken dataToken)
        {
            #region 数据解析
            int remainingBytesToProcess = redvArgs.BytesTransferred;
            bool needPostAnother = true;
            do
            {
                if (dataToken.prefixBytesDone < 4)
                {
                    remainingBytesToProcess = prefixHandler.HandlePrefix(redvArgs, dataToken, remainingBytesToProcess);
                    if (dataToken.prefixBytesDone == 4 && (dataToken.messageLength > ushort.MaxValue || dataToken.messageLength < 0))
                    {
                        needPostAnother = false;
                        HandleCloseSocket(redvArgs, "protocol error", false);
                        break;
                    }
                    //if (logger.IsDebugEnabled) logger.DebugFormat("处理消息头，已接收长度字节[{0}]", dataToken.prefixBytesDone);
                    if (remainingBytesToProcess == 0) break;
                }

                remainingBytesToProcess = messageHandler.HandleMessage(redvArgs, dataToken, remainingBytesToProcess);

                if (dataToken.IsMessageReady)
                {
                    //if (logger.IsDebugEnabled) logger.DebugFormat("完整封包 长度[{0}],总传输[{1}],剩余[{2}]", dataToken.messageLength, ioEventArgs.BytesTransferred, remainingBytesToProcess);

                    messagequeue.Add(new MessageArgs(redvArgs.AcceptSocket, dataToken.byteArrayForPrefix[3], dataToken.byteArrayForMessage));

                    if (remainingBytesToProcess != 0)
                    {
                        //if (logger.IsDebugEnabled) logger.DebugFormat("重置缓冲区,buffskip指针[{0}]。", dataToken.bufferSkip);
                        dataToken.Reset(false);
                    }
                }
                else
                {
                    //if (logger.IsDebugEnabled) logger.DebugFormat("不完整封包 长度[{0}],总传输[{1}],已接收[{2}]", dataToken.messageLength, ioEventArgs.BytesTransferred, dataToken.messageBytesDone);
                }
            } while (remainingBytesToProcess != 0);
            #endregion

            return needPostAnother;
        }

        private void PostSend(SocketAsyncEventArgs sendArgs)
        {
            DataToken dataToken = (DataToken)sendArgs.UserToken;
            if (dataToken.messageLength - dataToken.messageBytesDone <= this.socketSettings.SendSize)
            {
                sendArgs.SetBuffer(dataToken.bufferOffset, dataToken.messageLength - dataToken.messageBytesDone);
                Buffer.BlockCopy(dataToken.byteArrayForMessage, dataToken.messageBytesDone, sendArgs.Buffer, dataToken.bufferOffset, dataToken.messageLength - dataToken.messageBytesDone);
            }
            else
            {
                sendArgs.SetBuffer(dataToken.bufferOffset, this.socketSettings.SendSize);
                Buffer.BlockCopy(dataToken.byteArrayForMessage, dataToken.messageBytesDone, sendArgs.Buffer, dataToken.bufferOffset, this.socketSettings.SendSize);
            }
            try
            {
                var willRaiseEvent = sendArgs.AcceptSocket.SendAsync(sendArgs);
                if (!willRaiseEvent)
                {
                    ProcessSend(sendArgs);
                }
            }
            catch (ObjectDisposedException)
            {
                messagequeue.Add(new ErrorArgs(sendArgs.AcceptSocket, new Exception("connection drop(PostSend)"), SocketError.HostDown));
                ReleaseSendArgs(sendArgs);
            }
        }

        private void ProcessSend(SocketAsyncEventArgs sendArgs)
        {
            DataToken dataToken = (DataToken)sendArgs.UserToken;
            if (sendArgs.SocketError != SocketError.Success)
            {
                messagequeue.Add(new ErrorArgs(sendArgs.AcceptSocket, new Exception("send error"), sendArgs.SocketError));
                HandleCloseSocket(sendArgs, "send error", false);
            }
            else
            {
                dataToken.messageBytesDone += sendArgs.BytesTransferred;
                if (dataToken.messageBytesDone != dataToken.messageLength)
                {
                    PostSend(sendArgs);
                }
                else
                {
                    dataToken.Reset(true);

                    ExSocket exSocket;
                    if (!exSocketDict.TryGetValue(sendArgs.AcceptSocket, out exSocket))
                    {
                        ReleaseSendArgs(sendArgs);
                    }
                    else
                    {
                        var needSendList = exSocket.TryGetNeedSend();
                        if (needSendList == null)
                        {
                            ReleaseSendArgs(sendArgs);
                        }
                        else
                        {
                            int total = 0;
                            needSendList.ForEach(p => total += p.Length);
                            byte[] buffer = new byte[total];
                            int idx = 0;
                            for (int i = 0; i < needSendList.Count; i++)
                            {
                                var b = needSendList[i];
                                Buffer.BlockCopy(b, 0, buffer, idx, b.Length);
                                idx += b.Length;
                            }

                            dataToken.byteArrayForMessage = buffer;
                            dataToken.messageLength = buffer.Length;
                            PostSend(sendArgs);
                        }
                    }
                }
            }
        }

        public void SendData(Socket socket, byte[] data, int offset, int count, byte head)
        {
            byte[] buffer = new byte[count + 4];
            byte[] countBytes = BitConverter.GetBytes(count);
            buffer[0] = countBytes[0];
            buffer[1] = countBytes[1];
            buffer[2] = countBytes[2];
            buffer[3] = head;
            Buffer.BlockCopy(data, offset, buffer, 4, count);

            ExSocket exSocket;
            if (!exSocketDict.TryGetValue(socket, out exSocket)) throw new SocketException((int)SocketError.Shutdown);

            if (exSocket.CheckCanDirectSend(buffer))
            {
                SocketAsyncEventArgs sendArgs;
                sendArgsPool.TryPop(out sendArgs);
                sendArgs.AcceptSocket = socket;
                DataToken dataToken = (DataToken)sendArgs.UserToken;
                dataToken.byteArrayForMessage = buffer;
                dataToken.messageLength = buffer.Length;
                PostSend(sendArgs);
            }
        }
        public void SendData(Socket socket, byte[] data, byte head)
        {
            SendData(socket, data, 0, data.Length, head);
        }

        public void PostMessage(Func<object, Task> action, object state)
        {
            messagequeue.Add(new ActionArgs(action, state));
        }

        public void CloseSocket(Socket socket, string errMsg)
        {
            lock (socket)
            {
                try
                {
                    var ep = socket.RemoteEndPoint;
                    ExSocket tmp;
                    exSocketDict.TryRemove(socket, out tmp);
                    if (logger.IsDebugEnabled) logger.DebugFormat("closing connection {0} {1}", ep, errMsg);
                    messagequeue.Add(new CloseArgs(socket, SocketError.Success, errMsg));

                    socket.Close();
                    maxConnectionsEnforcer.Release();
                }
                catch { }
            }
        }
        private void HandleCloseSocket(SocketAsyncEventArgs ioEventArgs, string errMsg, bool isrecv)
        {
            var socket = ioEventArgs.AcceptSocket;
            lock (socket)
            {
                try
                {
                    var ep = socket.RemoteEndPoint;
                    ExSocket tmp;
                    exSocketDict.TryRemove(socket, out tmp);
                    if (logger.IsDebugEnabled) logger.DebugFormat("closing connection {0} {1}({2}) {3}", ep, ioEventArgs.SocketError, (int)ioEventArgs.SocketError, errMsg);
                    messagequeue.Add(new CloseArgs(socket, ioEventArgs.SocketError, errMsg));

                    socket.Close();
                    maxConnectionsEnforcer.Release();
                }
                catch { }
            }

            if (isrecv) ReleaseRecvArgs(ioEventArgs);
            else ReleaseSendArgs(ioEventArgs);
        }
    }
}
