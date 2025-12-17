using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class OpenNotificationForClubCommand : IRequest<ServiceResponse<string>>
    {
        public Guid ClubId { get; set; }
    }
}
