using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.ActiveDetail.Query
{
    public class GetActiveClubsFor30DaysQuery : IRequest<ServiceResponse<long>>
    {
        public GetActiveClubsFor30DaysQuery() { }
    }
}