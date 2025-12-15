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
            var mail = new MailMessage()
            {
                From = new MailAddress(configuration["MailSettings:Username"], "UniTrack", Encoding.UTF8),
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHtml
            };
            mail.To.Add(to);

            // Dosya ekleme işlemi
            if (attachments != null)
            {
                foreach (var (stream, fileName) in attachments)
                {
                    // stream -> dosya içeriği, fileName -> kullanıcıya görünen isim
                    mail.Attachments.Add(new Attachment(stream, fileName));
                }
            }

            using var smtp = new SmtpClient(configuration["MailSettings:SmtpHost"],
            int.Parse(configuration["MailSettings:SmtpPort"]))
            {
                Credentials = new NetworkCredential(configuration["MailSettings:Username"], configuration["MailSettings:Password"]),
                EnableSsl = true,
            };
            await smtp.SendMailAsync(mail);
        }

    }
}