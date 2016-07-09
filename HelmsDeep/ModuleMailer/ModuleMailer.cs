using HelmsDeepCommon;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModuleMailer
{
    public class ModuleMailer : BaseModule
	{
		private static Logger log = LogManager.GetCurrentClassLogger();
		object lockObject = new object();
		SendMail mail;

		protected override void InitInternal()
		{
			CheckParam("login");
			CheckParam("password");
			CheckParam("addresses");

			CheckParam("reportsDir");

			mail = new SendMail(Parameters["login"], Parameters["password"],
				Parameters["login"] + "@gmail.com",
				Parameters["addresses"].Split(new char[] { ' ' }, 
					StringSplitOptions.RemoveEmptyEntries));
		}

		public override void Execute(DataRecorder recorder)
		{
			if (!Monitor.TryEnter(lockObject))
			{
				log.Warn("Задача работы с почтой уже запущена, запуск второго экземпляра остановлен");
				return;
			}

			log.Info("Модуль работы с почтой");
			string reportsDir = Path.Combine(BaseDir, Parameters["reportsDir"]);

			var dirEn = Directory.EnumerateFiles(reportsDir);
			foreach(var filename in dirEn)
			{
				if (!filename.EndsWith(".report"))
					continue;

				try
				{
					string filepath = Path.Combine(reportsDir, filename);
					string text = File.ReadAllText(filepath);
					mail.Send("Отчет из деревни " + filename, text);
					log.Info($"Отчет {filename} отправлен, удаляем...");
					File.Delete(filepath);
					log.Info("Файл удален");
				}
				catch(Exception ex)
				{
					log.Error($"Ошибка отправки отчета {filename}: {ex.Message}");
				}
			}
			Monitor.Exit(lockObject);

		}

		
	}
}
