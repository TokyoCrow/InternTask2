using InternTask2.Core.Services.Abstract;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace InternTask2.Core.Services.Concrete
{

    public class InboxMailRU : ISendEmail
    {
        public async Task<bool> Send(string mailText, string addressee, string subject)
        {
            using (var smtp = new SmtpClient())
            using (var message = new MailMessage())
            {
                message.Bcc.Add(new MailAddress(addressee));
                message.Subject = subject;
                message.Body = mailText.ToString();
                smtp.EnableSsl = true;
                try
                {
                    await smtp.SendMailAsync(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
            return true;
        }
    }
}
