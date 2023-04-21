using MyServer.Database.Base;
using MyServer.Database.Model;
using System.Collections.Generic;

namespace MyServer.Database.Cache
{
    public class ScheduleCache : ICache
    {
        private ScheduleDB<Schedule> Schedule = new ScheduleDB<Schedule>();
        private Dictionary<string, string> data = new Dictionary<string, string>();

        public override void Init()
        {
            Schedule.Init();
        }

        public override void Start()
        {
            List<Schedule> schedules = Schedule.QueryAll();
            data.Clear();
            foreach (var s in schedules)
            {
                data.Add(s.cronkey, s.cron);
            }
        }

        public override void Exit()
        {
        }

        public string GetCron(string key)
        {
            return data[key];
        }
    }
}
