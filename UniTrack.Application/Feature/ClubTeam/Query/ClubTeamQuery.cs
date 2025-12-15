using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.ClubTeam;

namespace UniTrack.Application.Feature.ClubTeam.Query
{
    public class ClubTeamQuery : IRequest<ServiceResponse<List<ClubTeamResponseDTO>>>
    {
        public Guid ClubId { get; set; }
        public ClubTeamQuery(Guid ClubId)
        {
            this.ClubId = ClubId;
        }
        public ClubTeamQuery() { }
    }
}
