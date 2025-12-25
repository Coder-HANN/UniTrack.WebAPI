using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Notification;

namespace UniTrack.Application.Feature.Notification.Query
{
    public class GetClubAllNotificationQuery: IRequest<ServiceResponse<List<NotificationListResponse>>>
    {
        public GetClubAllNotificationQuery() { }

    }
}
