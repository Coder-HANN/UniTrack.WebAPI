using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class MarkUserNotificationAsReadCommand : IRequest<ServiceResponse<string>>
    {
        public Guid NotificationId { get; set; }
    }
}