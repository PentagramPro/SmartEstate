using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelmsDeepCommon
{
    public class GlobalContext
    {
        public string ScheduleFile = "schedule.json";
        public string BaseDir;
        public string RecordsDir = "records";
        public string ReportsDir = "reports";
        public string LogsDir = "logs";
        public IController RemoteProcessor;

        public string ScheduleFileFull => Path.Combine(BaseDir, ScheduleFile);
        public string RecordsDirFull => Path.Combine(BaseDir, RecordsDir);
        public string ReportsDirFull => Path.Combine(BaseDir, ReportsDir);
        public string LogsDirFull => Path.Combine(BaseDir, LogsDir);


        public GlobalContext(string baseDir)
        {
            BaseDir = baseDir;
        }   
    }
}
