using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Timer
{
    public interface ISchedule
    {
        void Do(int i);
    }
}
