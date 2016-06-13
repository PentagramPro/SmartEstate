using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelmsDeepCommon;

namespace HelmsDeep.Model
{
    public class JobRecord
    {
        public int Index;
        public BaseModule Module;
        public string Assembly;
        public ScheduleJob JobParams;
    }
}
