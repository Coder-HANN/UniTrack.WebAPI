using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.ActiveDetail.Query
{
    public class GetActiveClubsFor180DaysQuery : IRequest<ServiceResponse<long>>
    {
        public GetActiveClubsFor180DaysQuery() { }
    }
}
