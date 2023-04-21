using MyServer.Database.Attri;

namespace MyServer.Database.Model
{
    [Table("t_test_account")]
    public class Account
    {
        [Key]
        public long id { get; set; }
        [Idx]
        public string user_id { get; set; }
        public string name { get; set; }
    }

    public class AccountDB<Account> : IDB<Account>
    {
        //public override void Init()
        //{
        //    base.Init();
        //    ShowAllSqls();
        //}
    }
}
