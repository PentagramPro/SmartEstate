using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ModuleMailer
{
    public class SendMail
    {
		MailSettings Settings;
		

		public SendMail(MailSettings settings)
		{
			Settings = settings;
		}

        public void Send(string subject, string text)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(Settings.From);
			foreach (var addr in Settings.Targets)
				mail.To.Add(addr);

            mail.Subject = subject;
            mail.Body = text;

            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = Settings.Credential;
            smtpServer.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };
            smtpServer.Send(mail);
        }
    }
}
