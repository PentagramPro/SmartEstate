using HelmsDeepCommon;
using NLog;
using S22.Imap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ModuleMailer
{
	public class MailChecker
	{
		private static Logger log = LogManager.GetCurrentClassLogger();
		MailSettings settings;
        GlobalContext glContext;

		public MailChecker(MailSettings settings, GlobalContext glContext)
		{
			this.settings = settings;
            this.glContext = glContext;
		}

		public void Check()
		{
			log.Info("Проверяем почту для адреса " + settings.From);
			using (ImapClient Client = new ImapClient("imap.gmail.com", 993,
			 settings.Credential.UserName, settings.Credential.Password,
			 AuthMethod.Login, true))
			{
				IEnumerable<uint> uids = Client.Search(SearchCondition.Unseen());
				IEnumerable<MailMessage> messages = Client.GetMessages(uids);
				foreach(var msg in messages)
				{
					log.Info(" тема: " + msg.Subject + " от: " + msg.From.Address);
					if(IsAllowedMail(msg.From.Address))
					{
						log.Info(" разрешенное письмо");
						ExecuteCommand(msg.From.Address, msg.Subject.Trim(), msg.Body);
					}
				} 
			} 
		}

		bool IsAllowedMail(string from)
		{
			return settings.Targets.Contains(from);
		}

		void ExecuteCommand(string from, string subject, string body)
		{
			if(!subject.Equals("команда", StringComparison.InvariantCultureIgnoreCase))
			{
				log.Error("Получено письмо, не являющееся командой. Тема письма: " + subject);
				return;
			}
            if(body.Length>100)
            {
                log.Error("Полученная команда слишком длинная, длина " + body.Length);
                return;
            }
            body = body.Trim();
            log.Info("Команда: " + body);

            ControllerResponse response = new ControllerResponse();
            glContext.RemoteProcessor.ExecuteRemoteCommand(body, response);

            log.Info("Готовим отправку результата выполнения команды");

            SendMail mailSender = new SendMail(settings);
            mailSender.SendSimple($"результат выполнения команды '{body}'", 
                response.ResposneBody, response.Attachments);


		}
	}
}
