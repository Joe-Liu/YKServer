using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyServer.Server.TcpSocket
{
    /// <summary>
    /// 接收创建Socket连接，创建Channel与之绑定
    /// </summary>
    public class TcpServer
    {
        public string name { get; set; }
        public int port { get; set; }
        public int MaxConnections { get; set; }
        public MyAcceptor acceptor { get; set; }

        private AutoResetEvent m_messageReceivedEvent;
        private object m_messageReceivedEventCreationLock = new object();

        public TcpServer(string name, int port, int MaxConnections)
        {
            this.name = name;
            this.port = port;
            this.MaxConnections = MaxConnections;
            this.acceptor = new MyAcceptor(port);
        }

        public AutoResetEvent MessageReceivedEvent
        {
            get
            {
                if (m_messageReceivedEvent == null)
                {
                    lock (m_messageReceivedEventCreationLock) // make sure we don't create more than one event object
                    {
                        if (m_messageReceivedEvent == null)
                            m_messageReceivedEvent = new AutoResetEvent(false);
                    }
                }
                return m_messageReceivedEvent;
            }
        }

        public void Start()
        {
            this.acceptor.Start(MaxConnections);
        }

        public void Shutdown(string bye)
        {

        }

        public MyNetInMessage ReadMessage()
        {
            return null;
        }

        public void Recycle(MyNetInMessage message)
        {
            throw new Exception("Recycle MyNetInMessage");
        }
    }
}
