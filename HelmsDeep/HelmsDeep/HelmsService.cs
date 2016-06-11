using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using HelmsDeep.Model;
using log4net;
using log4net.Appender;
using log4net.Config;

namespace HelmsDeep
{
    public partial class HelmsService : ServiceBase
    {
        private string scheduleFile = "schedule.json";
        
        private string logFile = "logs/application.log";
        
        private Context context;
        private ILog log = LogManager.GetLogger(typeof (HelmsService));

        public HelmsService()
        {
            InitializeComponent();
            
        }

        protected override void OnStart(string[] args)
        {
        #if DEBUG
            System.Diagnostics.Debugger.Launch();
        #endif
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string logPath = Path.Combine(rootPath, logFile);
            Directory.CreateDirectory(logPath);
            

            var ap = new RollingFileAppender();
            ap.File = logPath;
            BasicConfigurator.Configure(ap);

            log.Info("Сервис запущен "+DateTime.Now.ToString(CultureInfo.CurrentCulture));
            context = new Context();
            string schedulePath = Path.Combine(rootPath, scheduleFile);
            
            context.Schedule = Schedule.Load(schedulePath);
            
            /*Schedule s = new Schedule();
            s.Job.Add(new ScheduleJob()
            {
                Assembly = "test.dll",Parameters =  new Dictionary<string, string>()
                {
                    {"param1","value1"},
                    {"param2","value2"},
                },
                Period = "1:30",
                StartType =  JobStartType.InTime
                
            });
            s.Save(schedulePath);*/
        }

        protected override void OnStop()
        {
        }
    }
}
