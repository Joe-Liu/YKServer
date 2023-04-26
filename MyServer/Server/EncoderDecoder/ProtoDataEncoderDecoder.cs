using Lidgren.Network;
using MyServer.Base;
using MyServer.Server.TcpSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Server.EncoderDecoder
{
    public class ProtoDataEncoderDecoder : IComponent
    {
        public string Decode(NetIncomingMessage msg)
        {
            string message = msg.ReadString();
            return message;
        }

        public string Decode(MyNetInMessage msg)
        {
            string message = msg.ReadString();
            return message;
        }

        public string Encode()
        {
            return null;
        }
    }
}
