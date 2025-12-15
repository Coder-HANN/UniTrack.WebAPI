using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.ActiveDetail.Query
{
    public class GetActiveUsersFor30DaysQuery : IRequest<ServiceResponse<long>>
    {
        public GetActiveUsersFor30DaysQuery() { }
    }
}
