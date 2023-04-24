using MyServer.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Server.Handler
{
    public abstract class IHttpHandler
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();

        public void Handle(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            switch (request.HttpMethod)
            {
                case "POST":
                    OnPost(request, HttpHelper.GetPostRequestContent(request), response);
                    break;
                case "GET":
                    OnGet(request, response);
                    break;
            }
        }

        protected abstract void OnGet(HttpListenerRequest request, HttpListenerResponse response);

        protected abstract void OnPost(HttpListenerRequest request, string postBody, HttpListenerResponse response);

        protected void Send(HttpListenerResponse response, string responseString)
        {
            try
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "application/json;charset=UTF-8";
                response.ContentEncoding = Encoding.UTF8;

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                System.IO.Stream stream = response.OutputStream;
                stream.Write(buffer, 0, buffer.Length);
                response.Close();
            }
            catch (Exception ex)
            {
                logger.Error($"Send Got Error {ex} ");
            }
        }
    }
}
