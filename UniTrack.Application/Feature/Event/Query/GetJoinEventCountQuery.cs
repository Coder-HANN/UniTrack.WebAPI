using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetJoinEventCountQuery : IRequest<ServiceResponse<int>>
    {
        public GetJoinEventCountQuery()
        {
        }
    }
}
