using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Timer.Attri
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class Cron : Attribute
    {
        private string key;

        public Cron(string key)
        {
            this.key = key;
        }

        public string Key { get => key; set => key = value; }
    }
}
