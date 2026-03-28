using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class MarkAllNotificationsAsReadForClubCommand : IRequest<ServiceResponse<string>>
    {
    }
}
