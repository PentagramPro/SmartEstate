using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelmsDeepCommon;
using Quartz;

namespace HelmsDeep.Model
{
    public class JobWrapper : IJob
    {
        public static Dictionary<int, JobRecord> Modules = new Dictionary<int, JobRecord>(); 
        public void Execute(IJobExecutionContext context)
        {
            int index = context.JobDetail.JobDataMap.GetIntValue("index");
            IModule mod = Modules[index].Module;
            mod.Execute();
        }
    }
}
