using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Server.Session
{
    public class ISession
    {
        public long playerId { get; protected set; }

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
        public virtual void SendMessage(string outMsg)
        {
            throw new Exception("SendMessage");
        }
    }
}
