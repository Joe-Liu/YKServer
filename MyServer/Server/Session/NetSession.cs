using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Server.Session
{
    public class NetSession : ISession
    {
        public NetConnection netConnection { get; protected set; }
        public NetServer netServer { get; protected set; }

        public NetSession(NetConnection netConnection, NetServer netServer)
        {
            this.netConnection = netConnection;
            this.netServer = netServer;
            this.playerId = -1L;
        }

        /// <summary>
        /// 发送文本消息内容
        /// </summary>
        /// <param name="outMsg"></param>
        public override void SendMessage(string outMsg)
        {
            NetOutgoingMessage msg = netServer.CreateMessage(outMsg);
            netServer.SendMessage(msg, netConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }
    }
}
