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
		string Login, Password;
		string From;
		IEnumerable<string> Targets;

		public SendMail(string login, string password, string from, IEnumerable<string> targets)
		{
			Login = login;
			Password = password;
			From = from;
			Targets = targets;
		}

        public void Send(string subject, string text)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(From);
			foreach (var addr in Targets)
				mail.To.Add(addr);

            mail.Subject = subject;
            mail.Body = text;

            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = new System.Net.NetworkCredential(Login,Password);
            smtpServer.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };
            smtpServer.Send(mail);
        }
    }
}
