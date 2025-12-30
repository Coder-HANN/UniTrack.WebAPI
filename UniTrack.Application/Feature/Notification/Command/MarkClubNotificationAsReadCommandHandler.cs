using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class MarkClubNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, ServiceResponse<string>>
    {
        private readonly IClubNotificationRepository clubNotificationRepository;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;

        public MarkClubNotificationAsReadCommandHandler(
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService,
            IClubNotificationRepository clubNotificationRepository)
        {
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
            this.clubNotificationRepository = clubNotificationRepository;
        }
        // Bildirimleri okundu işaretle
        public async Task<ServiceResponse<string>> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();

            if (clubId == null)
                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));

            
                var clubNotification = await clubNotificationRepository.GetByClubAndNotificationIdAsync(clubId.Value, request.NotificationId);

                if (clubNotification == null)
                    return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.NotificationNotFound));

                if (!clubNotification.IsRead)
                {
                    clubNotification.IsRead = true;
                    clubNotification.ReadDate = DateTimeOffset.UtcNow;
                    await clubNotificationRepository.UpdateAsync(clubNotification);
                }
            
            return ServiceResponse<string>.Success(null);
        }
    }
}