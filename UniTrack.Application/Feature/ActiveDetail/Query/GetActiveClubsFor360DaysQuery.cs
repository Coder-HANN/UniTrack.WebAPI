using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.ActiveDetail.Query
{
    public class GetActiveClubsFor360DaysQuery : IRequest<ServiceResponse<long>>
    {
        public GetActiveClubsFor360DaysQuery() { }
    }
}