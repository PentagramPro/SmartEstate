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
        public static Dictionary<string, IModule> Modules = new Dictionary<string, IModule>(); 
        public void Execute(IJobExecutionContext context)
        {
            IModule mod = Modules[context.JobDetail.JobDataMap["assembly"].ToString()];
            mod.Execute();
        }
    }
}
