using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class MarkAllNotificationsAsReadForUserCommand : IRequest<ServiceResponse<string>>
    {
    }
}
