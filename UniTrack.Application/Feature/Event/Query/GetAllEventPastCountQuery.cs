using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetAllEventPastCountQuery : IRequest<ServiceResponse<long>>
    {
        public GetAllEventPastCountQuery() { }
    }
}
