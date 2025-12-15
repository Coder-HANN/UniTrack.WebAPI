using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Ban;

namespace UniTrack.Application.Feature.Ban.Query
{
    public class GetBanedClubQuery : IRequest<ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>>>
    {
        public GetBanedClubQuery() { }
    }
}
