using MyServer.Base;
using MyServer.Logic.HttpHandler;
using MyServer.Server.Attri;
using MyServer.Server.Handler;
using MyServer.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Server.Component
{
    public class HttpDispatchComponent : IComponent
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        protected Dictionary<string, IHttpHandler> dispatcher;
        protected IHttpHandler defaultHttpHandler { get; set; }

        protected override void Init()
        {
            this.dispatcher = ClassUtil.Fetch<Dictionary<string, IHttpHandler>>();
            //收集所有的HttpHandler对象
            foreach (var type in ClassUtil.GetClasses("MyServer.Logic.HttpHandler"))
            {
                string path = null;
                System.Reflection.MemberInfo info = type;
                object[] attributes = info.GetCustomAttributes(false);
                for (int i = 0; i < attributes.Length; i++)
                {
                    if (attributes[i] is Http)
                    {
                        path = ((Http)attributes[i]).Path.ToLower();
                        break;
                    }
                }
                if (string.IsNullOrEmpty(path))
                {
                    logger.Error($"Can Not Find The AbsolutePath Of {type}");
                    continue;
                }
                this.dispatcher.Add(path, Activator.CreateInstance(type) as IHttpHandler);
            }

            IHttpHandler handler;
            if (this.dispatcher.TryGetValue("/404", out handler))
                defaultHttpHandler = handler;
        }

        public void Dispatch(HttpListenerContext context)
        {
            IHttpHandler handler;
            if (this.dispatcher.TryGetValue(context.Request.Url.AbsolutePath.ToLower(), out handler))
                handler.Handle(context);
            else
                defaultHttpHandler?.Handle(context);
        }
    }
}
