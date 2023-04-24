using MyServer.Base;
using MyServer.Server.Component;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyServer.Server
{
    public class HttpServer : IComponent
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public string Name { get; protected set; }
        public int Port { get; protected set; }
        public bool IsBackground { get; protected set; }
        public HttpListener listener { get; protected set; }
        public bool IsShutdown { get; protected set; }
        public CancellationTokenSource cancellation { get; protected set; }
        public HttpDispatchComponent httpDispatchComponent { get; protected set; }

        /// <summary>
        /// 创建一个Http服务
        /// </summary>
        /// <param name="name">服务名称</param>
        /// <param name="port">监听端口</param>
        /// <param name="isBackground">是否子线程执行</param>
        /// <returns>HttpServer</returns>
        public static HttpServer Create(string name, int port, bool isBackground = true)
        {
            var httpServer = new HttpServer();
            httpServer.Init(name, port, isBackground);
            return httpServer;
        }

        /// <summary>
        /// 启动Server
        /// </summary>
        public void Start()
        {
            if (IsBackground)
                new Thread(HttpServerStart).Start();
            else
                HttpServerStart();
        }

        /// <summary>
        /// 关闭Server
        /// </summary>
        public void ShutDown()
        {
            IsShutdown = true;
            cancellation.Cancel();
            listener.Stop();
        }

        protected void Init(string name, int port, bool isBackground)
        {
            if (!HttpListener.IsSupported)
            {
                logger.Error("Not Support HttpListener");
                return;
            }
            this.Name = name;
            this.Port = port;
            this.IsBackground = isBackground;

            listener = new HttpListener();
            listener.Prefixes.Add($"http://+:{port}/");

            httpDispatchComponent = AddComponent<HttpDispatchComponent>();
        }

        protected void HttpServerStart()
        {
            cancellation = new CancellationTokenSource();
            listener.Start();
            logger.Debug($"{this.Name} HttpServer Has Started! Port:{Port} , IsBackground:{IsBackground}");
            while (true)
            {
                Task<HttpListenerContext> task = listener.GetContextAsync();
                try { task.Wait(cancellation.Token); }catch (Exception ex) { }
                if (IsShutdown)
                    break;
                HttpListenerContext context = task.Result;
                httpDispatchComponent.Dispatch(context);
            }
            logger.Debug($"{this.Name} HttpServer Has Shutdown!");
        }
    }
}
