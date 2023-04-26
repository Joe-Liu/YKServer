using MyServer.Base;
using MyServer.MyThread;
using MyServer.Server.Component;
using MyServer.Server.EncoderDecoder;
using MyServer.Server.Session;
using NLog;
using System.Threading;
using MyServer.Server.TcpSocket;

namespace MyServer.Server
{
    /// <summary>
    /// TCP SocketServer
    /// </summary>
    public class TCPSocketServer : IComponent
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public TcpServer InServer { get; protected set; }
        public int WorkerSize { get; protected set; }
        public bool IsBackground { get; protected set; }
        public MyThreadPool ThreadPool { get; protected set; }
        public bool IsShutdown { get; protected set; }

        public SessionComponent sessionComponent { get; protected set; }
        public MessageDispatchComponent messageDispatchComponent { get; protected set; }
        public ProtoDataEncoderDecoder protoDataEncoderDecoder { get; protected set; }

        /// <summary>
        /// 构建一个SocketServer
        /// </summary>
        /// <param name="name">Server名字</param>
        /// <param name="port">监听端口号</param>
        /// <param name="MaxConnections">最大连接数,超过就会拒绝，默认1w</param>
        /// <param name="workerSize">收到消息后的处理线程数</param>
        /// <returns>SocketServer</returns>
        public static TCPSocketServer Create(string name, int port, int MaxConnections = 10000, int workerSize = 10, bool isBackground = true)
        {
            TCPSocketServer socketServer = new TCPSocketServer();
            socketServer.Init(name, port, MaxConnections, workerSize, isBackground);
            return socketServer;
        }

        /// <summary>
        /// 启动Server
        /// </summary>
        public void Start()
        {
            if (IsBackground)
                new Thread(ServerStart).Start();
            else
                ServerStart();
        }

        /// <summary>
        /// 关闭Server
        /// </summary>
        public void ShutDown()
        {
            IsShutdown = true;
            InServer.Shutdown("Server Shutdown");
            ThreadPool.Dispose();
        }


        protected void Init(string name, int port, int MaxConnections, int workerSize, bool isBackground)
        {
            WorkerSize = workerSize;
            IsBackground = isBackground;
            InServer = new TcpServer(name, port, MaxConnections);
            ThreadPool = new MyThreadPool(workerSize, OnMessageReceived);

            sessionComponent = AddComponent<SessionComponent>();
            messageDispatchComponent = AddComponent<MessageDispatchComponent>();
            protoDataEncoderDecoder = AddComponent<ProtoDataEncoderDecoder>();

            sessionComponent.tcpServer = InServer;
        }

        protected void ServerStart()
        {
            InServer.Start();
            string AppIdentifier = InServer.name;
            logger.Debug($"{AppIdentifier} Server Has Started! Port:{InServer.port} , IsBackground:{IsBackground}");

            while (true)
            {
                InServer.MessageReceivedEvent.WaitOne();
                MyNetInMessage msg;
                while ((msg = InServer.ReadMessage()) != null)
                {
                    var idx = msg.channel.GetHashCode() % WorkerSize;
                    ThreadPool.Enqueue(idx, msg);
                }
                if (IsShutdown)
                    break;
            }
            logger.Debug($"{AppIdentifier} Server Has Shutdown!");
        }

        protected void OnMessageReceived(object state)
        {
            MyNetInMessage msg = (MyNetInMessage)state;
            switch (msg.MessageType)
            {
                case MyNetInMessageType.Error:
                    logger.Warn($"Server Error:{msg.ReadString()}");
                    break;
                case MyNetInMessageType.StatusChanged:
                    MyTcpConnectionStatus status = (MyTcpConnectionStatus)msg.ReadByte();
                    sessionComponent.HandleNetConnectionStatus(msg.channel, status);
                    break;
                case MyNetInMessageType.Data:
                    string data = protoDataEncoderDecoder.Decode(msg);
                    ISession session = SessionManager.instance.GetSession(msg.channel.GetHashCode());
                    messageDispatchComponent.Dispatch(data, session);
                    break;
                default:
                    logger.Warn("Server Unhandled message type: " + msg.MessageType);
                    break;
            }
            InServer.Recycle(msg);
        }
    }
}
