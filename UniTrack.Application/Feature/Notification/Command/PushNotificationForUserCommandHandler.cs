
using UniTrack.Application.Abstraction.Services.Notification;
using UniTrack.Application.Common;
using MediatR;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class PushNotificationForUserCommandHandler: IRequestHandler<PushNotificationForUserCommand, ServiceResponse<bool>>
    {
        private readonly INotificationService notificationService;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;

        public PushNotificationForUserCommandHandler(
            INotificationService notificationService,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService)
        {
            this.notificationService = notificationService;
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
        }

        // Tek bir kullanıcıya bildirim gönderir 

        public async Task<ServiceResponse<bool>> Handle(PushNotificationForUserCommand request,CancellationToken cancellationToken)
        {
            var adminId = currentUserServices.CurrentUser();
            if (adminId == null)
            {
                return ServiceResponse<bool>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));
            }
            var role = currentUserServices.Role();
            if (role != Domain.Enums.Role.Admin)
            {
                return ServiceResponse<bool>.Fail(ValidationKeys.NotAuthorized);
            }

            await notificationService.SendDirectNotificationAsync(request.UserId,request.Message,request.Type,request.RelatedEntityId);

            return ServiceResponse<bool>.Success(null,true);
        }
    }
}


