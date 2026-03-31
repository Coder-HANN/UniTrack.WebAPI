using MediatR;

namespace UniTrack.Application.Feature.ActiveDetail.Query
{
    public class GetUserCountQuery : IRequest<long>
    {
        public GetUserCountQuery() { }
    }
}
