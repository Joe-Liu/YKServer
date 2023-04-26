using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.NetCore.Server;
using WebSocketSharp.NetCore;
using MyServer.Base;
using Logger = NLog.Logger;
using LogManager = NLog.LogManager;
using System.Threading;

namespace MyServer.Server
{
    /// <summary>
    /// 使用WebSocketSharp.NetCore构建WebSocketServer
    /// https://github.com/solana-fm/websocket-sharp
    /// </summary>
    public class WebSockServer : IComponent
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();

        public WebSockServer(int port)
        {
            var wssv = new WebSocketServer(9527);
            //wssv.Log.Level = LogLevel.Trace;
            wssv.Log.Level = LogLevel.Error;
            wssv.AddWebSocketService<Chat>("/Chat");

            wssv.Start();

            if (wssv.IsListening)
            {
                Console.WriteLine("Listening on port {0}, and providing WebSocket services:", wssv.Port);

                foreach (var path in wssv.WebSocketServices.Paths)
                    Console.WriteLine("- {0}", path);
            }

            Console.WriteLine("\nPress Enter key to stop the server...");
            Console.ReadLine();

            wssv.Stop();
        }

        public class Chat : WebSocketBehavior
        {
            private string _name;
            private static int _number = 0;
            private string _prefix;

            public Chat()
              : this(null)
            {
            }

            public Chat(string prefix)
            {
                _prefix = !string.IsNullOrEmpty(prefix) ? prefix : "anon#";
            }

            private string getName()
            {
                var name = Context.QueryString["name"];
                return !string.IsNullOrEmpty(name) ? name : _prefix + getNumber();
            }

            private static int getNumber()
            {
                return Interlocked.Increment(ref _number);
            }

            protected override void OnClose(CloseEventArgs e)
            {
                //Sessions.Broadcast(string.Format("{0} got logged off...", _name));
                //Send(string.Format("{0} got logged off...", _name));
            }

            protected override void OnMessage(MessageEventArgs e)
            {
                //Sessions.Broadcast(string.Format("{0}: {1}", _name, e.Data));
                Send(string.Format("{0}: {1}", _name, e.Data));
            }

            protected override void OnOpen()
            {
                _name = getName();
            }
        }
    }
}
