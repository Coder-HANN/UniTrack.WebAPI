using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Notification;

namespace UniTrack.Application.Feature.Notification.Query
{
    public class GetMyNotificationsQueryHandler: IRequestHandler<GetUserNotificationsQuery, ServiceResponse<List<NotificationListResponse>>>
    {
        private readonly IUserNotificationRepository userNotificationRepository;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ITargetNotificationRepository targetNotificationRepository;
        private readonly IClubRepository clubRepository;

        public GetMyNotificationsQueryHandler(
            IUserNotificationRepository userNotificationRepository,
            ICurrentUserServices currentUserServices,
            ITargetNotificationRepository targetNotificationRepository,
            IClubRepository clubRepository)
        {
            this.userNotificationRepository = userNotificationRepository;
            this.currentUserServices = currentUserServices;
            this.targetNotificationRepository = targetNotificationRepository;
            this.clubRepository = clubRepository;
        }

        public async Task<ServiceResponse<List<NotificationListResponse>>> Handle(GetUserNotificationsQuery request,CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();

            if (userId == null)
                return ServiceResponse<List<NotificationListResponse>>.Fail("Unauthorized");

            // 1️⃣ Bireysel bildirimler
            var userNotifications =await userNotificationRepository.GetUserNotificationsAsync(userId.Value);

            // 2️⃣ Kullanıcı bilgileri
            var cityId = currentUserServices.CityId();
            var universityId = currentUserServices.UniversityId();
            var departmentId = currentUserServices.DepartmentId();
            var clubIds = await clubRepository.GetUserClubIdsAsync(userId.Value);

            // 3️⃣ Target notification’ları runtime resolve et
            var targetNotifications =await targetNotificationRepository.GetMatchingNotificationsAsync(cityId,universityId,departmentId,clubIds);

            // 4️⃣ Mapping
            var result = new List<NotificationListResponse>();

            result.AddRange(userNotifications.Select(n => new NotificationListResponse
            {
                Title = n.Notification.Title ?? n.Notification.Type.ToString(),
                Message = n.Notification.Message,
                LogoUrl = n.Notification.LogoUrl,
                IsRead = n.IsRead,
                CreatedAt = n.Notification.CreatedAt,
                RelatedEntityId = n.Notification.RelatedEntityId
            }));

            result.AddRange(targetNotifications.Select(n => new NotificationListResponse
            {
                Title = n.Title ?? n.Type.ToString(),
                Message = n.Message,
                LogoUrl = n.LogoUrl,
                IsRead = false, // şimdilik
                CreatedAt = n.CreatedAt,
                RelatedEntityId = n.RelatedEntityId
            }));

            return ServiceResponse<List<NotificationListResponse>>
                .Success(null,result.OrderByDescending(x => x.CreatedAt).ToList());
        }
    }
}
