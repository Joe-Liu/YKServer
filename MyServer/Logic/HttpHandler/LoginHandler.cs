using MyServer.Server.Attri;
using MyServer.Server.Handler;
using System.Net;
using System.Threading;

namespace MyServer.Logic.HttpHandler
{
    [Http("/Login")]
    public class LoginHandler : IHttpHandler
    {
        protected override void OnGet(HttpListenerRequest request, HttpListenerResponse response)
        {
            string responseString = $"Hello world! LoginHandler Thread:{Thread.CurrentThread.ManagedThreadId}";
            Send(response, responseString);
        }

        protected override void OnPost(HttpListenerRequest request, string postBody, HttpListenerResponse response)
        {
            string responseString = $"Hello world! LoginHandler Thread:{Thread.CurrentThread.ManagedThreadId}";
            Send(response, responseString);
        }
    }
}
