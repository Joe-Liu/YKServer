using MyServer.Base;
using MyServer.Server.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyServer.Server.Component
{
    public class MessageDispatchComponent : IComponent
    {
        public void Dispatch(string inMsg, ISession session)
        {
            session.SendMessage($"{session.GetHashCode()} Server Received m11essage: {inMsg} Thread:{Thread.CurrentThread.ManagedThreadId}");
            session.SendMessage($"{session.GetHashCode()} Server Received m22essage: {inMsg} Thread:{Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
