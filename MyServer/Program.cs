using NLog;
using System;
using System.Transactions;
using System.Threading;
using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;
using System.Collections.Generic;
using MyServer.Database;
using MyServer.Timer;
using Lidgren.Network;
using MyServer.MyThread;
using MyServer.Server.Session;
using MyServer.Server;
using System.Text;
using System.Net;

namespace MyServer
{
    /// <summary>
    /// 每个线程有显式的线程ID
    /// Socket连接(每个玩家)Hash映射到几个线程里面去执行
    /// DB操作(延迟写入)放在独立几个(暂时1个)线程里面去执行
    /// 定时器回调的执行线程是多线程的
    /// 线程之间的消息传递？
    /// 将某个操作转到特定的线程上去执行？
    /// </summary>
    class Program
    {
        public static NLog.Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {

            Console.WriteLine("...");
            Console.ReadLine();
        }


        static void Load()
        {
            DBManager.instance.Load();
            //ScheduleManager.instance.Load();
            SessionManager.instance.Load();
        }

        static void Start()
        {
            DBManager.instance.Start();
            //ScheduleManager.instance.Start();
            SessionManager.instance.Start();
        }

        static void Exit()
        {
            DBManager.instance.Exit();
            //ScheduleManager.instance.Exit();
            SessionManager.instance.Exit();
        }

        /// <summary>
        /// Load的时候执行基本的初始化
        /// Start的时候执行启动服务的操作
        /// Exit的时候执行关闭服务的操作
        /// </summary>
        /// <param name="args"></param>
        //static void Main(string[] args)
        //{
        //    Load();
        //    Start();

        //    var GameServer = SocketServer.Create("game", 1234);
        //    GameServer.Start();
        //    var server = TCPSocketServer.Create("game", 1234);
        //    server.Start();
        //    var httpServer = HttpServer.Create("http", 8081);
        //    httpServer.Start();

        //    Console.WriteLine("任意键关闭服务");
        //    Console.ReadLine();

        //    GameServer.ShutDown();
        //    server.ShutDown();
        //    httpServer.ShutDown();
        //    Exit();

        //    Console.WriteLine("任意键退出应用");
        //    Console.ReadLine();

        //    Console.WriteLine("退出应用成功！");

        //}
    }
}
