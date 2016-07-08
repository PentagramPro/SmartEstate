using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelmsDeepCommon;
using NLog;
using Quartz;

namespace HelmsDeep.Model
{
    public class JobWrapper : IJob
    {
        public static Dictionary<int, JobRecord> Modules = new Dictionary<int, JobRecord>();
        private static Logger log = LogManager.GetCurrentClassLogger();
        public static Context ServiceContext;

        public void Execute(IJobExecutionContext context)
        {
            int index = context.JobDetail.JobDataMap.GetIntValue("index");
            try
            {
                
                IModule mod = Modules[index].Module;
                mod.Execute(ServiceContext.Recorder);
            }
            catch (KeyNotFoundException ex)
            {
                log.Error($"Работа с индексом {index} не найдена в списке");
            }
            catch (Exception ex)
            {
                log.Error($"При выполнении задания произошла ошибка");
            }
        }
    }
}
