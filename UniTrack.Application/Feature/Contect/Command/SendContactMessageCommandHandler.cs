using MediatR;
using Microsoft.Extensions.Configuration;
using System.Text;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Mail;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Contect.Command
{
    public class SendContactMessageCommandHandler
        : IRequestHandler<SendContactMessageCommand, ServiceResponse<string>>
    {
        private readonly IMailService mailService;
        private readonly IConfiguration configuration;
        private readonly ILocalizationService localizationService;

        public SendContactMessageCommandHandler(
            IMailService mailService,
            IConfiguration configuration,
            ILocalizationService localizationService)
        {
            this.mailService = mailService;
            this.configuration = configuration;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(
            SendContactMessageCommand request,
            CancellationToken cancellationToken)
        {
            var supportEmail = configuration.GetSection("MailSettings:FromEmail").Value;

            if (string.IsNullOrEmpty(supportEmail))
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.SupportEmailNotConfigured)
                };
            }

            var subject = $"[Contact] {request.Subject}";

            var bodyBuilder = new StringBuilder();
            bodyBuilder.AppendLine("<h3>Yeni İletişim Mesajı</h3>");
            bodyBuilder.AppendLine($"<p><strong>Ad Soyad:</strong> {request.Name}</p>");
            bodyBuilder.AppendLine($"<p><strong>Email:</strong> {request.Email}</p>");
            bodyBuilder.AppendLine("<hr>");
            bodyBuilder.AppendLine($"<p>{request.Message}</p>");

            await mailService.SendMailAsync(
                to: supportEmail,
                subject: subject,
                body: bodyBuilder.ToString(),
                isBodyHtml: true
            );

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = await localizationService.Get(ValidationKeys.ContactMessageSent)
            };
        }
    }
}
