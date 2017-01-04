using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using ExitGames.Logging;

namespace LilyServer.Helper
{
     public class MailHelper
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public string Subject { get; set; }
        public string MailFrom { get; set; }
        public string MailTo { get; set; }
        public string MailBody { get; set; }

        public void send() {

            SmtpClient client = new SmtpClient();

            MailMessage newmail = new MailMessage();
            MailFrom = "service@toufe.com";
            newmail.Subject = Subject;
            newmail.From = new MailAddress(MailFrom, "康熙德州扑克");
            newmail.To.Add(new MailAddress(MailTo));

            newmail.Body = MailBody;

            newmail.IsBodyHtml = true;

            try
            {
                client.Send(newmail);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }           
        }
    }
}
