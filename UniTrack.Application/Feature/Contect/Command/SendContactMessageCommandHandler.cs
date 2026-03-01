using MediatR;
using Microsoft.Extensions.Configuration;
using System.Text;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Mail;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Entities;

namespace UniTrack.Application.Feature.Contect.Command
{
    public class SendContactMessageCommandHandler: IRequestHandler<SendContactMessageCommand, ServiceResponse<string>>
    {
        private readonly IMailService mailService;
        private readonly IConfiguration configuration;
        private readonly ILocalizationService localizationService;
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUserRepository userRepository;

        public SendContactMessageCommandHandler(
            IMailService mailService,
            IConfiguration configuration,
            ILocalizationService localizationService,
            ICurrentUserServices currentUserServices,
            IUserRepository userRepository)
        {
            this.mailService = mailService;
            this.configuration = configuration;
            this.localizationService = localizationService;
            this.currentUserServices = currentUserServices;
            this.userRepository = userRepository;
        }

        public async Task<ServiceResponse<string>> Handle(SendContactMessageCommand request,CancellationToken cancellationToken)
        {
            // 1️⃣ Support mail kontrolü
            var supportEmail = configuration["MailSettings:FromEmail"];

            if (string.IsNullOrWhiteSpace(supportEmail))
            {
                return ServiceResponse<string>.Fail(
                    await localizationService.Get(ValidationKeys.SupportEmailNotConfigured));
            }

            // 2️⃣ Kullanıcı kimliği
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return ServiceResponse<string>.Fail(
                    await localizationService.Get(ValidationKeys.NotAuthorized));
            }

            // 3️⃣ Kullanıcıyı DB’den çek
            var user = await userRepository.GetByIdAsync(userId.Value);
            if (user == null)
            {
                return ServiceResponse<string>.Fail(
                    await localizationService.Get(ValidationKeys.UserNotFound));
            }

            // 4️⃣ Mail başlığı
            var subject = $"[Contact] {request.Subject}";

            // 5️⃣ HTML mail body
            var bodyBuilder = new StringBuilder();
            bodyBuilder.AppendLine("<h3>Yeni İletişim Mesajı</h3>");
            bodyBuilder.AppendLine($"<p><strong>Ad Soyad:</strong> {user.UserDetail.Name + " " + user.UserDetail.Surname}</p>");
            bodyBuilder.AppendLine($"<p><strong>Email:</strong> {user.Email}</p>");
            bodyBuilder.AppendLine("<hr>");
            bodyBuilder.AppendLine($"<p>{request.Message}</p>");

            // 6️⃣ Mail gönder
            await mailService.SendMailAsync(
                to: supportEmail,
                subject: subject,
                body: bodyBuilder.ToString(),
                isBodyHtml: true
            );

            // 7️⃣ Başarılı response
            return ServiceResponse<string>.Success(
                await localizationService.Get(ValidationKeys.ContactMessageSent));
        }
    }
}