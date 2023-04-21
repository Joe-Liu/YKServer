using MyServer.Database.Base;
using MyServer.Database.Model;
using NLog;
using System.Collections.Generic;

namespace MyServer.Database.Cache
{
    public class ConstCache : ICache, IStatic
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        private ConstDB<Const> Const = new ConstDB<Const>();
        private Dictionary<int, string> data = new Dictionary<int, string>();

        public override void Init()
        {
            Const.Init();
        }

        public override void Start()
        {
            List<Const> consts = Const.QueryAll();
            data.Clear();
            foreach(var c in consts)
            {
                data.Add(c.id, c.vaule);
            }
            logger.Info("Load t_s_testconst finish: " + data.Count);
        }

        public override void Exit()
        {
        }

        public void ReLoad()
        {
        }

        public string GetConst(int key)
        {
            return data[key];
        }
    }
}
