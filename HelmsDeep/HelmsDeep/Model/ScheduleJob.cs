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
        public JobStartType StartType;
        public int PeriodMinutes;
        public string Assembly;
        public Dictionary<string, string> Parameters;
    }
}
