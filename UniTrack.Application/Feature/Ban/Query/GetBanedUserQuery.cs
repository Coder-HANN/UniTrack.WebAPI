using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Ban;

namespace UniTrack.Application.Feature.Ban.Query
{
    public class GetBanedUserQuery : IRequest<ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>>>
    {
        public GetBanedUserQuery() { }
    }
}
