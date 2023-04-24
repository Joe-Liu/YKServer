using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Server.Session
{
    public class MySession
    {
        public NetConnection netConnection { get; protected set; }
        public NetServer netServer { get; protected set; }
        public long playerId { get; protected set; }

        public static MySession Create(NetConnection netConnection, NetServer netServer)
        {
            var session = new MySession();
            session.netConnection = netConnection;
            session.netServer = netServer;
            session.playerId = -1L;
            return session;
        }

        /// <summary>
        /// 绑定playerId
        /// </summary>
        /// <param name="playerId"></param>
        public void BindPlayerId(long playerId)
        {
            this.playerId = playerId;
        }

        /// <summary>
        /// 发送文本消息内容
        /// </summary>
        /// <param name="outMsg"></param>
        public void SendMessage(string outMsg)
        {
            NetOutgoingMessage msg = netServer.CreateMessage(outMsg);
            netServer.SendMessage(msg, netConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }
    }
}
