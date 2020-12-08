using InternTask2.Website.Properties;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace InternTask2.Website.Models
{
    interface ISendEmail
    {
        Task<bool> Send(string mailText, string addressee, string subject);
    }

    public class InboxMailRU : ISendEmail
    {
        readonly string host = "smtp.mail.ru";
        readonly int port = 587;
        public async Task<bool> Send(string mailText, string addressee, string subject)
        {
            try
            {
                using (SmtpClient smtp = new SmtpClient(host, port))
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(Settings.Default.AppEmail);
                    message.Bcc.Add(new MailAddress(addressee));
                    message.Subject = subject;
                    message.Body = mailText.ToString();
                    smtp.Credentials = new NetworkCredential(Settings.Default.AppEmail, Settings.Default.AppEmailPass);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }
    }
}
