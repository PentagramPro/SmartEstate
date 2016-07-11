using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelmsDeepCommon;
using NLog;

namespace ModuleReportGenerator
{
    public class ModuleReportGenerator : BaseModule
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        protected override void InitInternal()
        {
            CheckParam("template");
            CheckParam("reportsDir");
        }

        public override void Execute(DataRecorder recorder)
        {
            log.Info("--- Формируем отчет ---");
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

            proc.ProcessFile(Path.Combine(glContext.BaseDir,Parameters["template"]));

            string reportsDir = glContext.ReportsDirFull;

            Directory.CreateDirectory(reportsDir);

            string reportFile = Path.Combine(reportsDir,DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".report");

            File.WriteAllText(reportFile, proc.Report);
        }
    }
}
