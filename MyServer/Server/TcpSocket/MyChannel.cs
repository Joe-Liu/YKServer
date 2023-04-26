using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyServer.Server.TcpSocket
{
    public enum MyTcpConnectionStatus
    {
        None = 0,
        InitiatedConnect = 1,
        Connected = 2,
        Disconnecting = 3,
        Disconnected = 4
    }

    /// <summary>
    /// 持有Socket连接
    /// SocketAsyncEventArgs
    /// 发送出来的消息还是要有一些回收机制的
    /// </summary>
    public class MyChannel
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();

        private static int ChannelIdCounter = 0;

        public int ID { get; protected set; }
        public Socket socket { get; protected set; }
        public IPEndPoint RemoteAddress { get; set; }

        private SocketAsyncEventArgs innArgs = new SocketAsyncEventArgs();
        private SocketAsyncEventArgs outArgs = new SocketAsyncEventArgs();

        private readonly CircularBuffer recvBuffer = new CircularBuffer();
        private readonly CircularBuffer sendBuffer = new CircularBuffer();

        public MyChannel(Socket socket)
        {
            ID = Interlocked.Increment(ref ChannelIdCounter);
            logger.Debug($"-------MyChannel{ID}  Thread:{Thread.CurrentThread.ManagedThreadId}");
            this.socket = socket;
            this.socket.NoDelay = true;
            this.RemoteAddress = (IPEndPoint)socket.RemoteEndPoint;

            this.innArgs.Completed += this.OnComplete;
            this.outArgs.Completed += this.OnComplete;

            this.StartRecv();
            //this.StartSend();
        }

        private void StartRecv()
        {
            while (true)
            {
                try
                {
                    if (this.socket == null)
                    {
                        return;
                    }

                    int size = this.recvBuffer.ChunkSize - this.recvBuffer.LastIndex;
                    this.innArgs.SetBuffer(this.recvBuffer.Last, this.recvBuffer.LastIndex, size);
                }
                catch (Exception e)
                {
                    logger.Error($"tchannel error: {this.ID}\n{e}");
                    //this.OnError(ErrorCore.ERR_TChannelRecvError);
                    return;
                }

                if (this.socket.ReceiveAsync(this.innArgs))
                {
                    return;
                }
                this.HandleRecv(this.innArgs);
            }
        }

        private void OnRecvComplete(object o)
        {
            this.HandleRecv(o);

            if (this.socket == null)
            {
                return;
            }
            //this.StartRecv();
        }

        private void OnComplete(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    //OnConnectComplete(e);
                    //this.Service.ThreadSynchronizationContext.Post(() => OnConnectComplete(e));
                    break;
                case SocketAsyncOperation.Receive:
                    //logger.Debug($"异步OnComplete Receive完成: {this.ID}");
                    OnRecvComplete(e);
                    //this.Service.ThreadSynchronizationContext.Post(() => OnRecvComplete(e));
                    break;
                case SocketAsyncOperation.Send:
                    //this.Service.ThreadSynchronizationContext.Post(() => OnSendComplete(e));
                    break;
                case SocketAsyncOperation.Disconnect:
                    //this.Service.ThreadSynchronizationContext.Post(() => OnDisconnectComplete(e));
                    break;
                default:
                    throw new Exception($"socket error: {e.LastOperation}");
            }
        }

        private void HandleRecv(object o)
        {
            if (this.socket == null)
            {
                return;
            }
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;

            if (e.SocketError != SocketError.Success)
            {
                //this.OnError((int)e.SocketError);
                return;
            }

            if (e.BytesTransferred == 0)
            {
                //this.OnError(ErrorCore.ERR_PeerDisconnect);
                return;
            }

            this.recvBuffer.LastIndex += e.BytesTransferred;
            if (this.recvBuffer.LastIndex == this.recvBuffer.ChunkSize)
            {
                this.recvBuffer.AddLast();
                this.recvBuffer.LastIndex = 0;
            }

            //先用下sendBuffer测试
            int size = e.BytesTransferred;
            this.recvBuffer.Read(this.sendBuffer.Last, this.sendBuffer.LastIndex, size);
            try
            {
                var str = System.Text.Encoding.UTF8.GetString(this.sendBuffer.Last, this.sendBuffer.LastIndex, size);
                logger.Debug($"{ID}:  recv:  {str}  .");
                this.sendBuffer.LastIndex += size;
            }
            catch (Exception ex) { }

            //// 收到消息回调
            //while (true)
            //{
            //    // 这里循环解析消息执行，有可能，执行消息的过程中断开了session
            //    if (this.socket == null)
            //    {
            //        return;
            //    }
            //    try
            //    {

            //    }
            //    catch (Exception ee)
            //    {
            //        logger.Error($"ip: {this.RemoteAddress} {ee}");
            //        //this.OnError(ErrorCore.ERR_SocketError);
            //        return;
            //    }
            //}
        }

    }
}
