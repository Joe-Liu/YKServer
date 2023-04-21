using MyServer.Database.Attri;

namespace MyServer.Database.Model
{
    [Table("t_s_schedule")]
    public class Schedule
    {
        [Key]
        public string cronkey { get; set; }
        public string cron { get; set; }
    }

    public class ScheduleDB<Schedule> : IDB<Schedule>
    {
    }
}
