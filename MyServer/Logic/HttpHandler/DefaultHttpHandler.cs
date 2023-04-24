using MyServer.Server.Attri;
using MyServer.Server.Handler;
using System.Net;

namespace MyServer.Logic.HttpHandler
{
    [Http("/404")]
    public class DefaultHttpHandler : IHttpHandler
    {
        protected override void OnGet(HttpListenerRequest request, HttpListenerResponse response)
        {
            string responseString = $"<HTML><BODY>404</BODY></HTML>";
            Send(response, responseString);
        }

        protected override void OnPost(HttpListenerRequest request, string postBody, HttpListenerResponse response)
        {
        }
    }
}
