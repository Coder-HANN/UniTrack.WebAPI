using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.ClubTeam.Command
{
    public class DeleteClubTeamCommand : IRequest<ServiceResponse<string>>
    {
        public Guid ClubTeamId { get; set; }
    }
}
