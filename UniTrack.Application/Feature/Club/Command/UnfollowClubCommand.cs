using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Club.Command
{
    public class UnfollowClubCommand : IRequest<ServiceResponse<string>>
    {
        public Guid ClubId { get; set; }

    }
}
