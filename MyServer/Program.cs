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
        public static Logger logger = LogManager.GetCurrentClassLogger();


        static void Load()
        {
            DBManager.instance.Load();
            ScheduleManager.instance.Load();
        }

        static void Start()
        {
            DBManager.instance.Start();
            ScheduleManager.instance.Start();
        }

        static void Exit()
        {
            DBManager.instance.Exit();
            ScheduleManager.instance.Exit();
        }

        /// <summary>
        /// Load的时候执行基本的初始化
        /// Start的时候执行启动服务的操作
        /// Exit的时候执行关闭服务的操作
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var thread = new Thread(()=> {
                while (true)
                {
                    Thread.Sleep(10000);
                    logger.Warn("Do Thread***" + Thread.CurrentThread.ManagedThreadId);
                }
            });
            thread.Start();

            Load();
            Start();

            Console.WriteLine(".");
            Console.ReadLine();
        }
    }
}
