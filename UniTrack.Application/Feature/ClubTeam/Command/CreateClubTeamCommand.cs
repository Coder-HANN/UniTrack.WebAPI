using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.ClubTeam.Command
{
    public class CreateClubTeamCommand : IRequest<ServiceResponse<string>>
    {
        public Guid ClubId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
    }
}
