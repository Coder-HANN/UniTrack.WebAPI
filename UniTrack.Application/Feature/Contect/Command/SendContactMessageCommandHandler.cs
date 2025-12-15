using MediatR;
using Microsoft.Extensions.Configuration;
using System.Text;
using UniTrack.Application.Abstraction.Services.Mail;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Contect.Command
{
    public class SendContactMessageCommandHandler : IRequestHandler<SendContactMessageCommand, ServiceResponse<string>>
    {
        private readonly IMailService mailService;
        private readonly IConfiguration configuration;
        public SendContactMessageCommandHandler(
            IMailService mailService, 
            IConfiguration configuration)
        {
            this.mailService = mailService;
            this.configuration = configuration;
        }
        public async Task<ServiceResponse<string>> Handle(SendContactMessageCommand request, CancellationToken cancellationToken)
        {
            string supportEmail = configuration.GetSection("MailSettings:FromEmail").Value;

            if (string.IsNullOrEmpty(supportEmail))
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "Destek e-posta adresi yapılandırılmamış."
                };
            }

            // 2. Mail içeriğini oluşturun
            string subject = $"[Bize Ulaşın] Yeni Mesaj: {request.Subject}";

            // HTML gövde içeriği oluşturmak
            var bodyBuilder = new StringBuilder();
            bodyBuilder.AppendLine($"<h3>Kullanıcıdan Gelen Yeni Mesaj</h3>");
            bodyBuilder.AppendLine($"<p><strong>Gönderen Adı:</strong> {request.Name}</p>");
            bodyBuilder.AppendLine($"<p><strong>Gönderen E-postası:</strong> <a href='mailto:{request.Email}'>{request.Email}</a></p>");
            bodyBuilder.AppendLine($"<hr>");
            bodyBuilder.AppendLine($"<h4>Mesaj:</h4>");
            bodyBuilder.AppendLine($"<p>{request.Message}</p>");

            // 3. Mail servisini çağırın
                await mailService.SendMailAsync(
                    to: supportEmail,
                    subject: subject,
                    body: bodyBuilder.ToString(),
                    isBodyHtml: true
                );

            return new ServiceResponse<string>
            {
               IsSuccess = true,
               Message = "Mesajınız başarıyla gönderildi. En kısa sürede size geri dönüş yapacağız."
            };
        }
    }
}
