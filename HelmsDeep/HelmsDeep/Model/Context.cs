using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace HelmsDeep.Model
{
    public class Context
    {
        public Schedule Schedule { get; set; }
        public IScheduler Scheduler;
    }
}
