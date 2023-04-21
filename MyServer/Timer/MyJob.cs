using NLog;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyServer.Timer
{
    public class MyJob
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public IJobDetail job { get; set; }
        public ITrigger trigger { get; set; }

        [DisallowConcurrentExecution]
        public class AJob : IJob
        {
            public string CronKey { get; set; }
            public Task Execute(IJobExecutionContext context)
            {
                return Task.Run(() =>
                {
                    try
                    {
                        ScheduleManager.instance.DoScheduleNow(CronKey, 0);
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"{CronKey} Error:{ex.Message}");
                    }
                });
            }
        }

        public static MyJob Create(string cronKey, string cron)
        {
            var myJob = new MyJob();
            myJob.Init(cronKey, cron);
            return myJob;
        }

        public void Init(string cronKey, string cron)
        {
            job = JobBuilder.Create<AJob>()
                            .SetJobData(new JobDataMap() {
                                new KeyValuePair<string, object>("CronKey", cronKey)
                            })
                            .WithIdentity($"{cronKey}-job", $"{cronKey}-group")
                            .Build();

            trigger = TriggerBuilder.Create()
                .WithIdentity($"{cronKey}-trigger", $"{cronKey}-group")
                .StartNow()
                .WithCronSchedule(cron)
                .Build();
        }
    }
}
