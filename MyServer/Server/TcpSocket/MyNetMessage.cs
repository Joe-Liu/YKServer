using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Server.TcpSocket
{
    public enum MyNetInMessageType
    {
        Error = 0,
        StatusChanged = 1,
        Data = 2,
    }

    public class MyNetInMessage
    {
        public MyChannel channel { get; set; }
        public MyNetInMessageType MessageType { get; set; }

        public string ReadString()
        {
            return null;
        }

        public byte ReadByte()
        {
            return default(byte);
        }
    }

    public class MyNetOutMessage
    {
    }
}
