using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
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
            var fromEmail = configuration["MailSettings:FromEmail"];
            var username = configuration["MailSettings:Username"];
            var password = configuration["MailSettings:Password"];
            var host = configuration["MailSettings:SmtpHost"];

            if (!int.TryParse(configuration["MailSettings:SmtpPort"], out var port))
                throw new InvalidOperationException("MailSettings:SmtpPort invalid");

            if (string.IsNullOrWhiteSpace(fromEmail) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(host))
                throw new InvalidOperationException("MailSettings missing");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Öğrencity", fromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder();
            if (isBodyHtml)
                builder.HtmlBody = body;
            else
                builder.TextBody = body;

            if (attachments != null)
            {
                foreach (var (stream, fileName) in attachments)
                    await builder.Attachments.AddAsync(fileName, stream);
            }

            message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(username, password);
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MAIL ERROR TYPE: {ex.GetType().Name}");
                Console.WriteLine($"MAIL ERROR: {ex.Message}");
                Console.WriteLine($"MAIL INNER: {ex.InnerException?.Message}");
                Console.WriteLine($"MAIL STACK: {ex.StackTrace}");
                throw new InvalidOperationException("Mail sending failed: " + ex.Message, ex);
            }
        }
    }
}