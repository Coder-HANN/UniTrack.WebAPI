using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetClubFollowerCountQuery : IRequest<ServiceResponse<int>>
    {
        public GetClubFollowerCountQuery() { }
    }
}
