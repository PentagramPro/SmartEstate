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
using System.Threading;
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
        //private string scheduleFile = "schedule.json";
        
        //private string logFile = "logs";
        //private string recordsPath = "records";
        private static Logger log = LogManager.GetCurrentClassLogger();
        private PluginLoader<IModule> modulesLoader;
        private LocalContext context;
        public GlobalContext glContext;

        RemoteCommandProcessor rcProc;
        

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

            glContext = new GlobalContext(rootPath);
            rcProc = new RemoteCommandProcessor(glContext);

            glContext.RemoteProcessor = rcProc;
                        
            Directory.CreateDirectory(glContext.LogsDirFull);
            Directory.CreateDirectory(glContext.ReportsDirFull);
            Directory.CreateDirectory(glContext.RecordsDirFull);

            SetupLogger(glContext.LogsDirFull);

            
            log.Info("===================================");
            log.Info("Сервис запущен "+DateTime.Now.ToString(CultureInfo.CurrentCulture));
            context = new LocalContext();
            JobWrapper.ServiceContext = context;

            
            
            context.Scheduler= StdSchedulerFactory.GetDefaultScheduler();
            context.Recorder = new DataRecorder(glContext.RecordsDirFull);
            
            try
            {
                log.Info("Загружаем расписание");
                context.Schedule = Schedule.Load(glContext.ScheduleFileFull);

                log.Info("Загружаем исполняющие модули");
                modulesLoader = new PluginLoader<IModule>((type, name) =>
                {
                    var jobs = context.Schedule.Jobs.Where(p => p.Assembly == name);
                    List<IModule> res = new List<IModule>();
                    foreach (var job in jobs)
                    {
                        JobRecord rec = new JobRecord();
                        rec.Assembly = name;
                        rec.JobParams = job;
                        rec.Index = JobWrapper.Modules.Count;
                        rec.Module = (BaseModule)Activator.CreateInstance(type);
                        res.Add(rec.Module);
                        rec.Module.Name = name;
                        JobWrapper.Modules[rec.Index] = rec;
                    }
                    return res;
                });

                foreach (var rec in JobWrapper.Modules.Values)
                {
                    log.Info("Инициализируем "+rec.Assembly);
                    try
                    {
                        rec.Module.Init(rec.JobParams.Parameters, glContext);
                    }
                    catch (Exception e)
                    {
                        log.Error("Ошибка при инициализации: "+e.Message);
                    }
                }
                log.Info("------------------");
                log.Info("Назначаем работы");
                 
                Thread.Sleep(10000);
                foreach (var job in JobWrapper.Modules.Values)
                {
                    ScheduleJob(job);
                }

                context.Scheduler.Start();
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            /*
            Schedule s = new Schedule();
            s.Jobs.Add(new ScheduleJob()
            {
                Assembly = "test.dll",Parameters =  new Dictionary<string, string>()
                {
                    {"param1","value1"},
                    {"param2","value2"},
                },
                PeriodMinutes = 2,
                StartType =  JobStartType.InTime
                
            });
            s.Jobs.Add(new ScheduleJob()
            {
                Assembly = "test.dll",
                Parameters = new Dictionary<string, string>()
                {
                    {"param1","value1"},
                    {"param2","value2"},
                },
                PeriodMinutes = 2,
                StartType = JobStartType.InTime

            });
            s.Save(Path.Combine(rootPath,"test.json"));*/
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
            fileTarget.FileName = Path.Combine(logPath,"all.log");
            fileTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} |  ${message}";
            fileTarget.Encoding = Encoding.UTF8;
			fileTarget.ArchiveEvery = FileArchivePeriod.Day;
			fileTarget.ArchiveFileName = Path.Combine(logPath, "all.{########}.log");
			fileTarget.ArchiveNumbering = ArchiveNumberingMode.Date;
			fileTarget.MaxArchiveFiles = 10;

			var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
			
			config.AddTarget("file", fileTarget);
			config.LoggingRules.Add(rule2);

			fileTarget = new FileTarget();
			fileTarget.FileName = Path.Combine(logPath, "errors.log"); ;
			fileTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} |  ${message}";
			fileTarget.Encoding = Encoding.UTF8;

			rule2 = new LoggingRule("*", LogLevel.Error, fileTarget);

			config.AddTarget("file_errors", fileTarget);
			config.LoggingRules.Add(rule2);

			// Step 5. Activate the configuration
			LogManager.Configuration = config;
        }

        void ScheduleJob(JobRecord job)
        {
            try {
                IJobDetail jobDetail = JobBuilder.Create<JobWrapper>()
                    .UsingJobData("index", job.Index)
                    .Build();

                var tb = TriggerBuilder.Create();

                tb.WithCronSchedule(job.JobParams.CronString);
                
                log.Info("  модуль {0}, cron string: {1}", job.Assembly, job.JobParams.CronString);
                context.Scheduler.ScheduleJob(jobDetail, tb.Build());
            }
            catch(FormatException ex)
            {
                log.Error("Работа "+job.Assembly+": ошибка в cron string: " + ex.Message);
            }
        }
    }
}
