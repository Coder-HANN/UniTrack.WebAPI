using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.ActiveDetail.Query
{
    public class GetActiveClubsFor90DaysQuery : IRequest<ServiceResponse<long>>
    {
        public GetActiveClubsFor90DaysQuery() { }
    }
}