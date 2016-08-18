using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace ModuleMailer
{
    public class SendMail
    {
		MailSettings Settings;
        private static Logger log = LogManager.GetCurrentClassLogger();

        public SendMail(MailSettings settings)
		{
			Settings = settings;
		}

        MailMessage BuildMailMessage(string subject)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(Settings.From);
            foreach (var addr in Settings.Targets)
                mail.To.Add(addr);

            mail.Subject = subject;
            return mail;
        }

        void SendMailMessage(MailMessage mail, Action completed)
        {
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = Settings.Credential;
            smtpServer.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };
            smtpServer.Send(mail);

            if(completed!=null)
                smtpServer.SendCompleted += (o, ea) => completed();
            
        }

        Regex imgReg = new Regex(@"<img src='cid:([a-zA-Z0-9\-]+)'\/>");

        public void SendHtml(string subject, string text, string linkedResPath)
        {
            List<string> pathsToRemove = new List<string>();
            using (MailMessage mail = BuildMailMessage(subject))
            {

                mail.IsBodyHtml = true;

                AlternateView alternateView = AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Html);

                log.Info("подготавливаем вложения");
                
                var matches = imgReg.Matches(text);
                foreach (Match m in matches)
                {
                    string id = m.Groups[1].Value;
                    string path = Path.Combine(linkedResPath, id);

                    log.Info($"  найдено вложение {id} путь {path}");
                    LinkedResource lr = new LinkedResource(path);
                    lr.ContentId = id;

                    alternateView.LinkedResources.Add(lr);
                    pathsToRemove.Add(path);

                }

                mail.AlternateViews.Add(alternateView);


                SendMailMessage(mail, null);
            }
            pathsToRemove.ForEach(s => File.Delete(s));
        }

        public void SendSimple(string subject, string text, List<string> files)
        {
            MailMessage mail = BuildMailMessage(subject);

            mail.Body = text;
            if(files!=null)
            {
                files.ForEach(s => mail.Attachments.Add(new Attachment(s)));
            }

            SendMailMessage(mail,null);
        }
    }
}
