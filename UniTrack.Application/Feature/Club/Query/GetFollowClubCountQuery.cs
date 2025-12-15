using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetFollowClubCountQuery : IRequest<ServiceResponse<int>>
    {
        public GetFollowClubCountQuery() { }
    }
}
