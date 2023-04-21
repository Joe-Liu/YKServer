using MyServer.Database.Attri;

namespace MyServer.Database.Model
{
    [Table("t_s_testconst")]
    public class Const
    {
        [Key]
        public int id { get; set; }
        public string vaule { get; set; }
        public string desc { get; set; }
    }

    public class ConstDB<Const> : IDB<Const>
    {

    }
}
