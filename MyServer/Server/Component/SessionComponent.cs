using Lidgren.Network;
using MyServer.Base;
using MyServer.Server.Session;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Server.Component
{
    public class SessionComponent : IComponent
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();

        public NetServer netServer { get; set; }

        /// <summary>
        /// 处理网络连接状态变化，多线程
        /// </summary>
        public void HandleNetConnectionStatus(NetConnection netConnection, NetConnectionStatus status)
        {
            int hashCode = netConnection.GetHashCode();
            switch (status)
            {
                case NetConnectionStatus.InitiatedConnect: //启动连接
                    logger.Info($"Connection{hashCode} 启动连接");
                    break;
                case NetConnectionStatus.ReceivedInitiation: //接收初始化
                    logger.Info($"Connection{hashCode} 接收初始化");
                    break;
                case NetConnectionStatus.RespondedAwaitingApproval: //响应等待批准
                    logger.Info($"Connection{hashCode} 响应等待批准");
                    break;
                case NetConnectionStatus.RespondedConnect: //响应连接
                    logger.Info($"Connection{hashCode} 响应连接");
                    break;
                case NetConnectionStatus.Connected: //已连接
                    logger.Info($"Connection{hashCode} 已连接");
                    SessionManager.instance.AddConnect(netConnection, netServer);
                    break;
                case NetConnectionStatus.Disconnecting: //断开中
                    logger.Info($"Connection{hashCode} 断开中");
                    break;
                case NetConnectionStatus.Disconnected: //已断开
                    logger.Info($"Connection{hashCode} 已断开");
                    SessionManager.instance.RemoveConnect(netConnection);
                    break;
            }
        }
    }
}
