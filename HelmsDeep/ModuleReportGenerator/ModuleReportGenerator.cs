using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelmsDeepCommon;

namespace ModuleReportGenerator
{
    public class ModuleReportGenerator : BaseModule
    {
       
        protected override void InitInternal()
        {
            CheckParam("Template");
        }

        public override void Execute(DataRecorder recorder)
        {
            TemplateProcessor proc = new TemplateProcessor();
            recorder.ReadRecords((dt, name, values) =>
            {
                List<DataObject> list = null;
                if (proc.Data.ContainsKey(name))
                    list = proc.Data[name];
                else
                {
                    list = new List<DataObject>();
                    proc.Data[name] = list;
                }

                list.Add(new DataObject() {Time = dt,Values = values});
            });
            recorder.Rotate();

            proc.ProcessFile(Path.Combine(BaseDir,Parameters["Template"]));

        }
    }
}
