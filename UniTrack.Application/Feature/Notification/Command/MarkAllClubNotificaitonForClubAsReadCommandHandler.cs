using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class MarkAllClubNotificaitonForClubAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadForClubCommand, ServiceResponse<string>>
    {
        private readonly ILocalizationService localizationService;
        private readonly ICurrentUserServices currentUserServices;
        private readonly IClubNotificationRepository clubNotificationRepository;
        public MarkAllClubNotificaitonForClubAsReadCommandHandler(
            ILocalizationService localizationService,
            ICurrentUserServices currentUserServices,
            IClubNotificationRepository clubNotificationRepository)
        {
            this.localizationService = localizationService;
            this.currentUserServices = currentUserServices;
            this.clubNotificationRepository = clubNotificationRepository;
        }

        public async Task<ServiceResponse<string>> Handle(MarkAllNotificationsAsReadForClubCommand request, CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();

            if (clubId == null)
            {
                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));
            }

            bool isSuccess = await clubNotificationRepository.MarkAllAsReadAsync(clubId);

            if (!isSuccess)
            {
                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.AlreadyAllCommentReaded));
            }

            return ServiceResponse<string>.Success(null, await localizationService.Get(ValidationKeys.OperationSuccessful));
        }
    }
}
