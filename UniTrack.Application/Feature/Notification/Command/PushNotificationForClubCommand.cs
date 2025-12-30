using MediatR;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class PushNotificationForClubCommand : IRequest<ServiceResponse<bool>>
    {
        public Guid ClubId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public Guid? RelatedEntityId { get; set; }
    }
}
