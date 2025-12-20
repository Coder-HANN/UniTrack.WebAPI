using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetAllEventFeatureCountQuery : IRequest<ServiceResponse<long>>
    {
        public GetAllEventFeatureCountQuery() { }
    }
}
