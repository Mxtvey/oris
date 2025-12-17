using System.Net;
using System.Net.Mail;


namespace HttpServer.Services
{
    public static partial class EmailService
    {
        private const string SmtpHost = "smtp.gmail.com";
        private const int SmtpPort = 587;
        private const string SmtpUser = "matveysergeev1234@gmail.com";
        private const string SmtpPass = "*******";
        private const string FromAddr = "matveysergeev1234@gmail.com";
        private const string FromName = "HttpServer";
        public static async Task SendAsync(string to, string subject, string htmlBody)
        {
            using var message = new MailMessage(
                new MailAddress(FromAddr, FromName),
                new MailAddress(to))
            {
                Subject = subject,
                Body    = htmlBody,
                IsBodyHtml = true
            };

            using var smtp = new SmtpClient(SmtpHost, SmtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(SmtpUser, SmtpPass)
            };

            await smtp.SendMailAsync(message);
        }
    }
}

