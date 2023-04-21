using MyServer.Database;
using MyServer.Database.Cache;
using MyServer.Timer.Attri;
using MyServer.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using Quartz;
using Quartz.Impl;

namespace MyServer.Timer
{
    public class ScheduleManager : Singleton<ScheduleManager>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public Dictionary<string, ISchedule> schedules;
        public IScheduler scheduler { get; protected set; }

        public override void Init()
        {
            this.schedules = ClassUtil.Fetch<Dictionary<string, ISchedule>>();
            //收集所有的Schedule对象
            foreach (var type in ClassUtil.GetClasses("MyServer.Timer.Schedule"))
            {
                this.schedules.Add(GetCornKey(type), Activator.CreateInstance(type) as ISchedule);
            }

            ISchedulerFactory schedf = new StdSchedulerFactory();
            scheduler = schedf.GetScheduler().Result;
        }

        /// <summary>
        /// 获取这个Schedule的执行Cron
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal string GetCornKey(Type type)
        {
            object[] attributes = type.GetCustomAttributes(false);
            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i] is Cron)
                {
                    return ((Cron)attributes[i]).Key;
                }
            }
            logger.Error($"Can not find the cron in {type}");
            return null;
        }

        public override void Exit()
        {
        }

        public override void Load()
        {
        }

        /// <summary>
        /// 添加到定时器中
        /// </summary>
        public override void Start()
        {
            this.scheduler.Start();
            ScheduleCache scheduleCache = DBManager.instance.GetCache<ScheduleCache>();
            foreach (var s in this.schedules)
            {
                string cron = scheduleCache.GetCron(s.Key);
                MyJob myJob = MyJob.Create(s.Key, cron);
                this.scheduler.ScheduleJob(myJob.job, myJob.trigger).Wait();
                logger.Info($"ScheduleJob: {s.Key}");
            }
        }

        /// <summary>
        /// 直接执行某个Schedule，通过参数i指定更细节的内容
        /// </summary>
        /// <param name="cronKey"></param>
        /// <param name="i"></param>
        public void DoScheduleNow(string cronKey, int i)
        {
            if (this.schedules.ContainsKey(cronKey))
            {
                this.schedules[cronKey].Do(i);
            }
            else
            {
                logger.Error($"schedules doesn't has the schedule with key {cronKey}");
            }
        }
    }
}
