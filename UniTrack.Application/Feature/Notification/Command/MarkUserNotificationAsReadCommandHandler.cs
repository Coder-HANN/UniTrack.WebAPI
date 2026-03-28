using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class MarkUserNotificationAsReadCommandHandler: IRequestHandler<MarkUserNotificationAsReadCommand, ServiceResponse<string>>
    {
        private readonly IUserNotificationRepository userNotificationRepository;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;

        public MarkUserNotificationAsReadCommandHandler(
            IUserNotificationRepository userNotificationRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService)
        {
            this.userNotificationRepository = userNotificationRepository;
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
        }
        // Bildirimleri okundu işaretle
        public async Task<ServiceResponse<string>> Handle(MarkUserNotificationAsReadCommand request,CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();

            if (userId == null)
                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));

           
                var userNotification = await userNotificationRepository.GetByUserAndNotificationIdAsync(userId.Value, request.NotificationId);

                if (userNotification == null)
                    return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.NotificationNotFound));

                if (!userNotification.IsRead)
                {
                    userNotification.IsRead = true;
                    userNotification.ReadDate = DateTimeOffset.UtcNow;
                    await userNotificationRepository.UpdateAsync(userNotification);
                }
            
            return ServiceResponse<string>.Success(null);
        }
    }
}