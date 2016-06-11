using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace HelmsDeep.Model
{
    public class Schedule
    {
        public List<ScheduleJob> Jobs = new List<ScheduleJob>();

        public static Schedule Load(string path)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Deserialize<Schedule>(File.ReadAllText(path));
        }

        public void Save(string path)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            File.WriteAllText(path,json.Serialize(this));
        }
    }
}
