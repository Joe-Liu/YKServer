using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using NLog;
using System.Threading;

namespace MyServer.Server.TcpSocket
{
    public class MyAcceptor
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Socket acceptor;
        private readonly SocketAsyncEventArgs innArgs = new SocketAsyncEventArgs();

        public MyAcceptor(int port)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, port);
            this.acceptor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.acceptor.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this.acceptor.ReceiveBufferSize = 81920;
            this.innArgs.Completed += this.OnComplete;
            this.acceptor.Bind(ipEndPoint);
        }

        public void Start(int backlog)
        {
            //this.acceptor.Listen(backlog);
            this.acceptor.Listen();
            this.AcceptAsync();
        }

        private void OnComplete(object sender, SocketAsyncEventArgs e)
        {
            //logger.Debug($"--- 异步接收完成 ----OnComplete： {Thread.CurrentThread.ManagedThreadId}");
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    //logger.Debug($"-------SocketAsyncOperation.Accept");
                    this.OnAcceptComplete(e);
                    //SocketError socketError = e.SocketError;
                    //Socket acceptSocket = e.AcceptSocket;
                    //this.ThreadSynchronizationContext.Post(() => { this.OnAcceptComplete(socketError, acceptSocket); });
                    break;
                default:
                    throw new Exception($"socket error: {e.LastOperation}");
            }
        }

        private void OnAcceptComplete(SocketAsyncEventArgs e)
        {
            //logger.Debug($"--- 异步接收完成 ----OnAcceptComplete： {Thread.CurrentThread.ManagedThreadId}");

            if (this.acceptor == null)
            {
                return;
            }

            if (e.SocketError != SocketError.Success)
            {
                logger.Error($"accept error {e.SocketError}");
                return;
            }

            //logger.Debug($"{e.AcceptSocket == null}");
            //this.ThreadSynchronizationContext.Post(this.OnFinishAccept, e.AcceptSocket);

            OnFinishAccept(e.AcceptSocket);

            // 开始新的accept
            this.AcceptAsync();
        }

        private void OnAcceptComplete(SocketError socketError, Socket acceptSocket)
        {
            //logger.Debug($"-- 直接接收完成 -----OnAcceptComplete： {Thread.CurrentThread.ManagedThreadId}");
            if (this.acceptor == null)
            {
                return;
            }

            if (socketError != SocketError.Success)
            {
                logger.Error($"accept error {socketError}");
                return;
            }

            try
            {
                OnFinishAccept(acceptSocket);
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }

            // 开始新的accept
            this.AcceptAsync();
        }

        private void AcceptAsync()
        {
            //logger.Debug($"-- 新接收 -----AcceptAsync： {Thread.CurrentThread.ManagedThreadId}");
            this.innArgs.AcceptSocket = null;
            if (this.acceptor.AcceptAsync(this.innArgs))
            {
                return;
            }
            OnAcceptComplete(this.innArgs.SocketError, this.innArgs.AcceptSocket);
        }

        int cnt = 0;

        private void OnFinishAccept(object state)
        {
            Socket acceptSocket = state as Socket;
            //logger.Debug($"-------OnFinishAccept{++cnt}： {acceptSocket.GetHashCode()}");
            var channel = new MyChannel(acceptSocket);
        }
    }
}
