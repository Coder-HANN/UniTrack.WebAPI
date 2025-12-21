using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetAllClubEventJoinerCountQuery : IRequest<ServiceResponse<long>>
    {
        public GetAllClubEventJoinerCountQuery()
        {
        }
    }
}
