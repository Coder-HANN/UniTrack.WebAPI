using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetAllEventCountQuery : IRequest<ServiceResponse<long>>
    {
        public GetAllEventCountQuery() { }
    }
}
