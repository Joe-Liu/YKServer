using MyServer.Timer.Attri;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyServer.Timer.Schedule
{
    [Cron("every_day")]
    public class ScheduleDay : ISchedule
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public void Do(int i)
        {
            logger.Warn("Do ScheduleDay***" + Thread.CurrentThread.ManagedThreadId);
        }
    }
}
