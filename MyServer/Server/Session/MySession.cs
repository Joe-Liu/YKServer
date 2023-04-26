using MyServer.Server.TcpSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Server.Session
{
    public class MySession : ISession
    {
        public MyChannel channel { get; protected set; }
        public TcpServer tcpServer { get; protected set; }

        public MySession(MyChannel channel, TcpServer tcpServer)
        {
            this.channel = channel;
            this.tcpServer = tcpServer;
            this.playerId = -1L;
        }

        /// <summary>
        /// 发送文本消息内容
        /// </summary>
        /// <param name="outMsg"></param>
        public override void SendMessage(string outMsg)
        {

        }
    }
}
