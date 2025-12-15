using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetAllClubCountQuery : IRequest<ServiceResponse<long>>
    {
        public GetAllClubCountQuery() { }
    }
}
