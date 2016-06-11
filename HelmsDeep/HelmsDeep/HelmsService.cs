using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using CrypTool;
using HelmsDeep.Model;
using HelmsDeepCommon;
using NLog;
using NLog.Config;
using NLog.Targets;
using Quartz;
using Quartz.Impl;


namespace HelmsDeep
{
    
    public partial class HelmsService : ServiceBase
    {
        private string scheduleFile = "schedule.json";
        
        private string logFile = "logs/application.log";
        private static Logger log = LogManager.GetCurrentClassLogger();
        private PluginLoader<IModule> modulesLoader;
        private Context context;
        

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
            Directory.CreateDirectory(Path.GetDirectoryName(logPath));

            SetupLogger(logPath);

            
            log.Info("===================================");
            log.Info("Сервис запущен "+DateTime.Now.ToString(CultureInfo.CurrentCulture));
            context = new Context();
            string schedulePath = Path.Combine(rootPath, scheduleFile);

            context.Scheduler= StdSchedulerFactory.GetDefaultScheduler();

            try
            {
                log.Info("Загружаем расписание");
                context.Schedule = Schedule.Load(schedulePath);

                log.Info("Загружаем исполняющие модули");
                modulesLoader = new PluginLoader<IModule>((assembly, module) => module.Name=assembly, rootPath);

                foreach (var plugin in modulesLoader.Plugins)
                {
                    log.Info("Инициализируем "+plugin.Name);
                    plugin.Init();
                    JobWrapper.Modules[plugin.Name] = plugin;
                }

                log.Info("Назначаем работы");
                 
                foreach (var job in context.Schedule.Jobs)
                {
                    ScheduleJob(job);
                }

                context.Scheduler.Start();
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            /*Schedule s = new Schedule();
            s.Jobs.Add(new ScheduleJob()
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
            context.Scheduler.Shutdown();
        }


        void SetupLogger(string logPath)
        {
            // Step 1. Create configuration object 
            var config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 
            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // Step 3. Set target properties 
            fileTarget.FileName = logPath;
            fileTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
            fileTarget.Encoding = Encoding.UTF8;

            // Step 4. Define rules
            var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule2);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;
        }

        void ScheduleJob(ScheduleJob job)
        {
            IJobDetail jobDetail = JobBuilder.Create<JobWrapper>().UsingJobData("assembly",job.Assembly).Build();

            var tb = TriggerBuilder.Create();
            switch (job.StartType)
            {
                case JobStartType.Now:
                    tb.StartNow();
                    break;
                case JobStartType.InTime:
                    break;
            }
            tb.WithSimpleSchedule(x => x.WithIntervalInMinutes(job.PeriodMinutes).RepeatForever());

            context.Scheduler.ScheduleJob(jobDetail, tb.Build());
        }
    }
}
