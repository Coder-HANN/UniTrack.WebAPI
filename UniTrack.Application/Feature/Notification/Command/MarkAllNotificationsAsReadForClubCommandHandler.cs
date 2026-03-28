using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class MarkAllNotificationsAsReadForClubCommandHandler : IRequestHandler<MarkAllNotificationsAsReadForClubCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;
        private readonly IUserNotificationRepository userNotificationRepository;
        
        public MarkAllNotificationsAsReadForClubCommandHandler(
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService,
            IUserNotificationRepository userNotificationRepository)
        {
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
            this.userNotificationRepository = userNotificationRepository;
        }

        public async Task<ServiceResponse<string>> Handle(MarkAllNotificationsAsReadForClubCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();

            if (userId == null)
            {
                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));
            }

            bool isSuccess = await userNotificationRepository.MarkAllAsReadAsync(userId);

            if (!isSuccess)
            {
                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.AlreadyAllCommentReaded));
            }

            return ServiceResponse<string>.Success(null, await localizationService.Get(ValidationKeys.OperationSuccessful));
        }
    }
}
