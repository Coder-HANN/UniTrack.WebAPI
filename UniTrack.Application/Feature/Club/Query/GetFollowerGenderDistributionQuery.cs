using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Club;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetFollowerGenderDistributionQuery: IRequest<ServiceResponse<List<GenderDistributionDTO>>>
    {
        public GetFollowerGenderDistributionQuery() { }
    }
}