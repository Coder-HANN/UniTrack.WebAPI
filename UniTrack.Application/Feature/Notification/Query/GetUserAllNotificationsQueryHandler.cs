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

        public GetMyNotificationsQueryHandler(
            IUserNotificationRepository userNotificationRepository,
            ICurrentUserServices currentUserServices)
        {
            this.userNotificationRepository = userNotificationRepository;
            this.currentUserServices = currentUserServices;
        }

        public async Task<ServiceResponse<List<NotificationListResponse>>> Handle(GetUserNotificationsQuery request,CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();

            if (userId == null)
            {
                return ServiceResponse<List<NotificationListResponse>>.Fail("Unauthorized");
            }

            var notifications = await userNotificationRepository.GetUserNotificationsAsync(userId.Value);

            var result = notifications.Select(x => new NotificationListResponse
            {
                Title = x.Notification.Title,
                Message = x.Notification.Message,
                LogoUrl = x.Notification.LogoUrl,
                DisplayType = !string.IsNullOrEmpty(x.Notification.Title)
                    ? x.Notification.Title
                    : x.Notification.Type.ToString(),
                IsRead = x.IsRead,
                CreatedAt = x.Notification.CreatedAt,
                RelatedEntityId = x.Notification.RelatedEntityId
            })
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

            return ServiceResponse<List<NotificationListResponse>>.Success(null,result);
        }
    }
}
