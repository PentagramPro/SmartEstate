using System;
using System.Collections.Generic;
using System.IO;
using HelmsDeepCommon;
using HelmsTests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModuleReportGenerator;

namespace HelmsTests
{
    [TestClass]
    public class TestModuleReportGenerator
    {
        [TestMethod]
        public void TestLoadModule()
        {
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            GlobalContext glContext = new GlobalContext(rootPath);
            var parameters = new Dictionary<string, string>() { { "template","" }};

            var module = new ModuleReportGenerator.ModuleReportGenerator();

            module.Init(parameters,glContext);
        }

        [TestMethod]
        public void TestGenerateReport()
        {
            TestFramework framework = new TestFramework();
            framework.PrepareTestDir(Resources.TestData,Resources.TestTemplate);
            DataRecorder recorder = new DataRecorder(framework.TestDataDir);

            var parameters = new Dictionary<string, string>() { { "template", framework.TestTemplateFile } };

            var module = new ModuleReportGenerator.ModuleReportGenerator();

            module.Init(parameters, framework.GlobalContext);
            module.Execute(recorder);

            using (var file = framework.OpenReportFile())
            {
                Assert.AreEqual("temp: 20", file.ReadLine());
                Assert.AreEqual("temp: 60", file.ReadLine());
                Assert.AreEqual("temp: 40", file.ReadLine());
            }
        }
    }
}
