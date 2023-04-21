using MyServer.Database.Base;
using MyServer.Database.Model;

namespace MyServer.Database.Cache
{
    public class AccoutCache : ICache
    {
        private AccountDB<Account> Account = new AccountDB<Account>();

        public override void Init()
        {
            Account.Init();
        }

        public override void Start()
        {
        }

        public override void Exit()
        {
        }
    }
}
