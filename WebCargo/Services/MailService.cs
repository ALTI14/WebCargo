using System.Net;
using System.Net.Mail;

namespace WebCargo.Services
{
    public static class MailService
    {
        public static IConfiguration? Configuration { get; set; }

        public static void SendMail(
    string email,
    string subject,
    string body)
        {
            string mailAddress =
                Configuration!["Mail:Address"]!;

            string mailPassword =
                Configuration!["Mail:Password"]!;

            MailMessage mail =
                new MailMessage();

            mail.From =
                new MailAddress(mailAddress);

            mail.To.Add(email);

            mail.Subject =
                subject;

            mail.Body =
                body;

            SmtpClient smtp =
                new SmtpClient(
                    "smtp.gmail.com",
                    587
                );

            smtp.Credentials =
                new NetworkCredential(
                    mailAddress,
                    mailPassword
                );

            smtp.EnableSsl = true;

            smtp.Send(mail);
        }
    }
}