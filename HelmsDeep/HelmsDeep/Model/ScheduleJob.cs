using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace HelmsDeep.Model
{
    public class ScheduleJob
    {
        public string CronString;
        public string Assembly;
        public Dictionary<string, string> Parameters;
    }
}
