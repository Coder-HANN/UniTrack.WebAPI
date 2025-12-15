using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Club.Command
{
    public class FollowClubCommand : IRequest<ServiceResponse<string>>
    {
        public Guid ClubId { get; set; }
    }
}
