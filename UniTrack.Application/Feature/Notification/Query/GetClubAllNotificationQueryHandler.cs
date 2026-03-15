using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Notification;

namespace UniTrack.Application.Feature.Notification.Query
{
    public class GetClubAllNotificationQueryHandler : IRequestHandler<GetClubAllNotificationQuery, ServiceResponse<List<NotificationListResponse>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;
        private readonly IClubNotificationRepository clubNotificationRepository;
        private readonly ITargetNotificationRepository targetNotificationRepository;

        public GetClubAllNotificationQueryHandler (
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService,
            IClubNotificationRepository clubNotificationRepository,
            ITargetNotificationRepository targetNotificationRepository)
        {
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
            this.clubNotificationRepository = clubNotificationRepository;
            this.targetNotificationRepository = targetNotificationRepository;
        }

        public async Task<ServiceResponse<List<NotificationListResponse>>> Handle(GetClubAllNotificationQuery request, CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();

            if (clubId == null)
                return ServiceResponse<List<NotificationListResponse>>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));

            var clubNotification = await clubNotificationRepository.GetClubAllNotification(clubId.Value);

            var cityId = currentUserServices.CityId();
            var universityId = currentUserServices.UniversityId();

            var targetNotification = await targetNotificationRepository.GetMatchingNotificationsForClubAsync(cityId, universityId);

            var result = new List<NotificationListResponse>();

            result.AddRange(clubNotification.Select(n => new NotificationListResponse
            {
                Title = n.Notification.Title ?? n.Notification.Type.ToString(),
                Message = n.Notification.Message,
                LogoUrl = n.Notification.LogoUrl,
                IsRead = n.IsRead,
                CreatedAt = n.Notification.CreatedAt,
                RelatedEntityId = n.Notification.RelatedEntityId,
                NotificationType = n.Notification.Type
            }));

            result.AddRange(targetNotification.Select(n => new NotificationListResponse
            {
                Title = n.Notification.Title ?? n.Notification.Type.ToString(),
                Message = n.Notification.Message,
                LogoUrl = n.Notification.LogoUrl,
                IsRead = false, // şimdilik
                CreatedAt = n.Notification.CreatedAt,
                RelatedEntityId = n.Notification.RelatedEntityId,
                NotificationType = n.Notification.Type
            }));

            return ServiceResponse<List<NotificationListResponse>>.Success(null, result.OrderByDescending(x => x.CreatedAt).ToList());
        }
    }
}