using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.ActiveDetail.Query
{
    public class GetActiveUsersFor90DaysQuery : IRequest<ServiceResponse<long>>
    {
        public GetActiveUsersFor90DaysQuery() { }
    }
}
