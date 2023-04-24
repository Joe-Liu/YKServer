using Lidgren.Network;
using MyServer.Base;
using MyServer.MyThread;
using MyServer.Server.Component;
using MyServer.Server.EncoderDecoder;
using MyServer.Server.Session;
using NLog;
using System.Threading;

namespace MyServer.Server
{
    /// <summary>
    /// SocketServer
    /// </summary>
    public class SocketServer : IComponent
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public NetServer InServer { get; protected set; }
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
        public static SocketServer Create(string name, int port, int MaxConnections = 10000, int workerSize = 10, bool isBackground = true)
        {
            SocketServer socketServer = new SocketServer();
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
            NetPeerConfiguration config = new NetPeerConfiguration(name);
            config.Port = port;
            config.MaximumConnections = MaxConnections;
            config.UseMessageRecycling = true;
            InServer = new NetServer(config);
            ThreadPool = new MyThreadPool(workerSize, OnMessageReceived);

            sessionComponent = AddComponent<SessionComponent>();
            messageDispatchComponent = AddComponent<MessageDispatchComponent>();
            protoDataEncoderDecoder = AddComponent<ProtoDataEncoderDecoder>();

            sessionComponent.netServer = InServer;
        }

        protected void ServerStart()
        {
            InServer.Start();
            string AppIdentifier = InServer.Configuration.AppIdentifier;
            logger.Debug($"{AppIdentifier} Server Has Started! Port:{InServer.Configuration.Port} , IsBackground:{IsBackground}");

            while (true)
            {
                InServer.MessageReceivedEvent.WaitOne();
                NetIncomingMessage msg;
                while ((msg = InServer.ReadMessage()) != null)
                {
                    var idx = msg.SenderConnection.GetHashCode() % WorkerSize;
                    ThreadPool.Enqueue(idx, msg);
                }
                if (IsShutdown)
                    break;
            }
            logger.Debug($"{AppIdentifier} Server Has Shutdown!");
        }

        protected void OnMessageReceived(object state)
        {
            NetIncomingMessage msg = (NetIncomingMessage)state;
            switch (msg.MessageType)
            {
                case NetIncomingMessageType.Error:
                    logger.Warn($"Server Error:{msg.ReadString()}");
                    break;
                case NetIncomingMessageType.ConnectionApproval:
                    logger.Warn($"Server ConnectionApproval:{msg.ReadString()}");
                    break;
                case NetIncomingMessageType.StatusChanged:
                    NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                    sessionComponent.HandleNetConnectionStatus(msg.SenderConnection, status);
                    break;
                case NetIncomingMessageType.UnconnectedData:
                    logger.Warn($"Server UnconnectedData:{msg.ReadString()}");
                    break;
                case NetIncomingMessageType.Receipt:
                    logger.Warn($"Server Receipt:{msg.ReadString()}");
                    break;
                case NetIncomingMessageType.Data:
                    string data = protoDataEncoderDecoder.Decode(msg);
                    MySession session = SessionManager.instance.GetSession(msg.SenderConnection.GetHashCode());
                    messageDispatchComponent.Dispatch(data, session);
                    break;
                case NetIncomingMessageType.DiscoveryRequest:
                case NetIncomingMessageType.DiscoveryResponse:
                    logger.Warn($"Server Discovery message type: {msg.MessageType}  message:{msg.ReadString()}");
                    break;
                case NetIncomingMessageType.VerboseDebugMessage:
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.ErrorMessage:
                    logger.Warn($"Server Error message type: {msg.MessageType}  message:{msg.ReadString()}");
                    break;
                default:
                    logger.Warn("Server Unhandled message type: " + msg.MessageType);
                    break;
            }
        }
    }
}
