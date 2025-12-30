using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Entities;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class PushNotificationForClubCommandHandler: IRequestHandler<PushNotificationForClubCommand, ServiceResponse<bool>>
    {
        private readonly INotificationRepository notificationRepository;
        private readonly IClubNotificationRepository clubNotificationRepository;
        private readonly ILocalizationService localizationService;
        private readonly ICurrentUserServices currentUserServices;

        public PushNotificationForClubCommandHandler(
            INotificationRepository notificationRepository,
            IClubNotificationRepository clubNotificationRepository,
            ILocalizationService localizationService,
            ICurrentUserServices currentUserServices)
        {
            this.notificationRepository = notificationRepository;
            this.clubNotificationRepository = clubNotificationRepository;
            this.localizationService = localizationService;
            this.currentUserServices = currentUserServices;
        }

        public async Task<ServiceResponse<bool>> Handle(PushNotificationForClubCommand request,CancellationToken cancellationToken)
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

            // 1️⃣ Notification (1 tane)
            var notification = new Domain.Entities.Notification
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Message = request.Message,
                Type = request.Type,
                RelatedEntityId = request.RelatedEntityId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await notificationRepository.AddAsync(notification);

            // 2️⃣ ClubNotification (1 tane)
            var clubNotification = new ClubNotification
            {
                Id = Guid.NewGuid(),
                ClubId = request.ClubId,
                NotificationId = notification.Id,
                IsRead = false
            };

            await clubNotificationRepository.AddAsync(clubNotification);

            return ServiceResponse<bool>.Success(null,true);
        }
    }
}
