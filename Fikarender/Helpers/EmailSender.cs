using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Fikarender.Data;

namespace Helpers
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            string smtp = "";
            string userName = "";
            string password = "";
            string displayName = "";

            using (var db = new ApplicationDbContext())
            {
                /*var c = db.Config.Select(a => new { a.MailDisplayName, a.MailPassword, a.MailSmtpDomain, a.MailUserName }).AsNoTracking().FirstOrDefault();
                smtp = c.MailSmtpDomain;
                userName = c.MailUserName;
                password = c.MailPassword;
                displayName = c.MailDisplayName;*/
            }

            MailMessage mail = new MailMessage();
            using (SmtpClient SmtpServer = new SmtpClient(smtp))
            {
                mail.From = new MailAddress(userName, displayName);
                mail.To.Add(email);
                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new NetworkCredential(userName, password);
                SmtpServer.EnableSsl = true;
                await SmtpServer.SendMailAsync(mail);
            }
        }
    }
}
