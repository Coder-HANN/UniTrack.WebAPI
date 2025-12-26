using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Notification;

namespace UniTrack.Application.Feature.Notification.Query
{
    public class GetClubAllNotificationQueryHandler : IRequestHandler<GetClubAllNotificationQuery, ServiceResponse<List<NotificationListResponse>>>
    {

        public Task<ServiceResponse<List<NotificationListResponse>>> Handle(GetClubAllNotificationQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}