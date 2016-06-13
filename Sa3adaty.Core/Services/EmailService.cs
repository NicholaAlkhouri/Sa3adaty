using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Sa3adaty.Core.Services
{
    public class EmailService
    {
        private int port;
        private string from_email;
        private string username;
        private string password;
        private string smtp_server;

        public EmailService()
        {
            this.from_email = ConfigurationManager.AppSettings["smtpFrom"];
            this.username = ConfigurationManager.AppSettings["smtpUsername"];
            this.password = ConfigurationManager.AppSettings["smtpPassword"];
            this.port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]);
        }

        public EmailService(string from_email, string username, string password,int port )
        {
            this.from_email = from_email;
            this.username = username;
            this.password = password;
            this.port = port;
        }

        public bool SendHtmlEmail(string to_addresses,string subject, string message)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(smtp_server);

            mail.From = new MailAddress(this.from_email,"موقع سعادتي");
            mail.To.Add(to_addresses);
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            string htmlBody;

            htmlBody = message;

            mail.Body = htmlBody;

            SmtpServer.Port = this.port ;
            SmtpServer.Credentials = new System.Net.NetworkCredential(this.username ,this.password );
            SmtpServer.EnableSsl = false;

            SmtpServer.Send(mail);

            return true;
        }

        public string LoadTemplate(string template_path)
        {
            if (File.Exists( HttpContext.Current.Server.MapPath("~" + GetTemplatesDirectory() + template_path)))
            {
                return File.ReadAllText(HttpContext.Current.Server.MapPath("~" + GetTemplatesDirectory() + template_path));
            }
            else
            {
                return "";
            }
        }

        public string GetTemplatesDirectory()
        {
            return "/EmailTemplates/";
        }

    }
}
