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
            string reportsDir = glContext.ReportsDirFull;

            log.Info("--- Формируем отчет ---");
            string reportId = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            TemplateProcessor proc = new TemplateProcessor(reportsDir, reportId);
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
            //recorder.Rotate();

            proc.ProcessFile(Path.Combine(glContext.BaseDir,Parameters["template"]));

            

            Directory.CreateDirectory(reportsDir);

            string reportFile = Path.Combine(reportsDir, reportId + ".report");

            File.WriteAllText(reportFile, proc.Report);
        }
    }
}
