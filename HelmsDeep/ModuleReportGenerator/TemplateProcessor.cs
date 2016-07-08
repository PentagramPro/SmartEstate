using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HelmsDeepCommon.Exceptions;
using ModuleReportGenerator.Exceptions;
using NLog;

namespace ModuleReportGenerator
{
    public class TemplateProcessor
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public class MinAvgMax
        {
            public float Min=0, Avg=0, Max=0;
            public int Count = 1;
        }
        public Dictionary<string, List<DataObject>> Data { get; }
            = new Dictionary<string, List<DataObject>>();

       public Dictionary<string,Dictionary<int,MinAvgMax>> precalc
            = new Dictionary<string, Dictionary<int, MinAvgMax>>();

        public string Report { get; private set; }

        public void ProcessFile(string path)
        {
            StringBuilder resultingReport = new StringBuilder();
            if (!File.Exists(path))
            {
                string text = "Файл шаблона не найден: " + path;
                log.Error(text);
                throw new ModuleExecutionException(text);
            }

            // Предварительная обработка данных
            Precalculate();

            // Обрабатываем шаблон
            var tpl = File.OpenText(path);


            string line;
            int lineNum = 1;
            while ((line = tpl.ReadLine()) != null)
            {
                resultingReport.AppendLine(ProcessLine(line, lineNum));
                lineNum++;
            }

            tpl.Close();

            Report = resultingReport.ToString();
        }

        void Precalculate()
        {
            precalc.Clear();
            foreach (var key in Data.Keys)
            {
                var data = Data[key];
                Dictionary<int,MinAvgMax> subRec = new Dictionary<int, MinAvgMax>();

                foreach (var dataObject in data)
                {
                    for (int i = 0; i < dataObject.Values.Length; i++)
                    {
                        float val = dataObject.Values[i];

                        if (subRec.ContainsKey(i))
                        {
                            var subRecData = subRec[i];
                            if (subRecData.Min > val)
                                subRecData.Min = val;
                            if (subRecData.Max < val)
                                subRecData.Max = val;
                            subRecData.Avg = (subRecData.Avg*subRecData.Count + val)/(subRecData.Count + 1);
                            subRecData.Count++;
                        }
                        else
                            subRec[i] = new MinAvgMax
                            {
                                Min = val,
                                Avg = val,
                                Max = val,
                                Count = 1
                            };
                    }
                }

                if (subRec.Count > 0)
                    precalc[key] = subRec;
            }
        }
        Regex regFunc = new Regex(@"{(\w[\w\d]*)\s*\(([^\)]*)\)}");
        string ProcessLine(string line, int lineNum)
        {
            var ms = regFunc.Matches(line);
            return regFunc.Replace(line, m =>
            {
                return ExecuteFunction(m.Groups[1].Value,lineNum, m.Groups[2].Value.Split(','));
            });

        }

        string ExecuteFunction(string name, int lineNum, string[] parameters)
        {
            if(parameters.Length<2)
                throw new TemplateException($"Недостаточно параметров у функции {name} в строке {lineNum}");
            
            string module = parameters[0];
            int index = 0;
            if (!int.TryParse(parameters[1], out index))
                throw new TemplateException($"Функция {name}, строка {lineNum}: второй параметр должен быть числом, а не {parameters[1]}");

            try
            {
                switch (name)
                {
                    case "Max":
                        return precalc[module][index].Max.ToString();
                    case "Min":
                        return precalc[module][index].Min.ToString();
                    case "Avg":
                        return precalc[module][index].Avg.ToString();
                    case "Count":
                        return precalc[module][index].Count.ToString();
                }
            }
            catch (KeyNotFoundException ex)
            {
                return $"({name}, {index}: нет данных)";                
            }
            catch (Exception ex)
            {
                throw new TemplateException($"Функция {name}, строка {lineNum}: ошибка выполнения");
            }

            throw new TemplateException($"Функция {name}, строка {lineNum}: такая функция не найдена");
        }
    }
}
