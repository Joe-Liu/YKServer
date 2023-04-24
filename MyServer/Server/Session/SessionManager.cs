using Lidgren.Network;
using MyServer.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Server.Session
{
    public class SessionManager : Singleton<SessionManager>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static Dictionary<int, NetConnection> connectionDic = new Dictionary<int, NetConnection>();
        private static Dictionary<int, MySession> sessionDic = new Dictionary<int, MySession>();
        private static Dictionary<long, MySession> playerSessionDic = new Dictionary<long, MySession>();

        private object sessionLock = new object();
        private object playerLock = new object();

        public override void Init()
        {
            connectionDic.Clear();
            sessionDic.Clear();
            playerSessionDic.Clear();
        }

        /// <summary>
        /// 添加一个网络连接，自动创建对应的Session
        /// 多线程操作
        /// </summary>
        /// <param name="netConnection"></param>
        public void AddConnect(NetConnection netConnection, NetServer netServer)
        {
            lock (sessionLock)
            {
                int HashCode = netConnection.GetHashCode();
                if (connectionDic.ContainsKey(HashCode))
                {
                    logger.Error($"Already Has This NetConnection {HashCode}");
                    return;
                }
                connectionDic.Add(HashCode, netConnection);
                var session = MySession.Create(netConnection, netServer);
                sessionDic.Add(HashCode, session);
            }
        }

        /// <summary>
        /// 移除一个网络连接，删除对应的Session
        /// 如果Session上绑定有用户的话，移除用户相关的数据（移除方式？）
        /// 多线程操作
        /// </summary>
        /// <param name="netConnection"></param>
        public void RemoveConnect(NetConnection netConnection)
        {
            long playerId = -1L;
            lock (sessionLock)
            {
                int HashCode = netConnection.GetHashCode();
                if (!connectionDic.ContainsKey(HashCode))
                {
                    logger.Error($"Don't Have This NetConnection {HashCode}");
                    return;
                }
                connectionDic.Remove(HashCode);
                if (sessionDic[HashCode] != null)
                    playerId = sessionDic[HashCode].playerId;
                sessionDic.Remove(HashCode);
            }

            if (playerId > 0)
            {
                lock (playerLock)
                {
                    playerSessionDic.Remove(playerId);
                }
            }
        }

        /// <summary>
        /// 根据NetConnection HashCode获取Session
        /// </summary>
        /// <param name="connectionHash"></param>
        /// <returns></returns>
        public MySession GetSession(int connectionHash)
        {
            if (sessionDic.ContainsKey(connectionHash))
                return sessionDic[connectionHash];
            return null;
        }

        /// <summary>
        /// 根据PlayerID获取Session
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public MySession GetSession(long playerId)
        {
            if(playerSessionDic.ContainsKey(playerId))
                return playerSessionDic[playerId];
            return null;
        }

        /// <summary>
        /// 为当前Session绑定一个PlayerID
        /// </summary>
        /// <param name="session"></param>
        /// <param name="playerId"></param>
        public void SessionBindPlayerID(MySession session, long playerId)
        {
            lock (playerLock)
            {
                session.BindPlayerId(playerId);
                if (playerSessionDic.ContainsKey(playerId))
                {
                    logger.Error($"Already Has This PlayerSession {playerId}");
                    return;
                }
                playerSessionDic.Add(playerId, session);
            }
        }

        public override void Exit()
        {
            logger.Debug($"SessionManager Exit");
        }

        public override void Load()
        {
            logger.Debug($"SessionManager Load");
        }

        public override void Start()
        {
            logger.Debug($"SessionManager Start");
        }
    }
}
