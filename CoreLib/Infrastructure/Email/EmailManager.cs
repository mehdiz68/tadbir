using System;
using System.Net.Mail;

namespace CoreLib.Infrastructure.Email
{
    public class EmailManager
    {
        MailAddress FromAddress = new MailAddress("");
        MailAddress ToAddress = new MailAddress("");
        public bool Success{get;set;}

        public EmailManager(string from, string to, string pass, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(from);
                mail.To.Add(new MailAddress(to));
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.Priority = MailPriority.High;
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                SmtpClient smtp = new SmtpClient();
                smtp.Timeout *= 2;
                smtp.Credentials = new System.Net.NetworkCredential(from, pass);
                smtp.Send(mail);
            }
            catch (Exception)
            {
                Success=false;
            }
        }

    }

}
