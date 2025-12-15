using MediatR;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetAllEventFeatureCountQuery : IRequest<long>
    {
        public GetAllEventFeatureCountQuery() { }
    }
}
