using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using UniTrack.Application.Abstraction.Services.Mail;

namespace UniTrack.Infrastructure.Services
{
    public class MailService : IMailService
    {
        private readonly IConfiguration configuration;

        public MailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendMailAsync(
            string to,
            string subject,
            string body,
            bool isBodyHtml = true,
            List<(Stream Stream, string FileName)> attachments = null)
        {
            var fromEmail = configuration["MailSettings:Username"];
            var password = configuration["MailSettings:Password"];
            var host = configuration["MailSettings:SmtpHost"];

            if (!int.TryParse(configuration["MailSettings:SmtpPort"], out var port))
                throw new InvalidOperationException("MailSettings:SmtpPort invalid");

            if (string.IsNullOrWhiteSpace(fromEmail) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(host))
                throw new InvalidOperationException("MailSettings missing");

            using var mail = new MailMessage
            {
                From = new MailAddress(fromEmail, "Öğrencity", Encoding.UTF8),
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHtml
            };

            mail.To.Add(to);

            if (attachments != null)
            {
                foreach (var (stream, fileName) in attachments)
                {
                    mail.Attachments.Add(new Attachment(stream, fileName));
                }
            }

            using var smtp = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true,
                Timeout = 10000
            };

            try
            {
                await smtp.SendMailAsync(mail);
            }
            catch (SmtpException ex)
            {
                // burada log atılmalı
                throw new InvalidOperationException("Mail sending failed", ex);
            }
        }
    }
}