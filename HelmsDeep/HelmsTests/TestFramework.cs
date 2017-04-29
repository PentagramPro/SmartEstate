using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HelmsDeepCommon;
using HelmsTests.Properties;

namespace HelmsTests
{
    public class TestFramework
    {
        string BaseDir;
        public GlobalContext GlobalContext { get; private set; }
        public TestFramework()
        {
            BaseDir = AppDomain.CurrentDomain.BaseDirectory;
            GlobalContext = new GlobalContext(TestDirectory);
        }

        public void PrepareTestDir(string testData, string testTemplate)
        {
            if(Directory.Exists(TestDirectory))
                Directory.Delete(TestDirectory,true);
            Directory.CreateDirectory(TestDirectory);
            Directory.CreateDirectory(TestDataDir);

            File.WriteAllText(TestDataFile, testData);
            File.WriteAllText(TestTemplateFile, testTemplate);
        }

        public StreamReader OpenReportFile()
        {
            var file = Directory.GetFiles(GlobalContext.ReportsDirFull, "*.report").First();
            return File.OpenText(file);
        }
        public string TestDirectory => Path.Combine(BaseDir, "test");

        public string TestDataDir => Path.Combine(TestDirectory, "data");
        public string TestDataFile => Path.Combine(TestDirectory, "data/records.dat");
        public string TestTemplateFile => Path.Combine(TestDirectory, "template.txt");
    }
}
