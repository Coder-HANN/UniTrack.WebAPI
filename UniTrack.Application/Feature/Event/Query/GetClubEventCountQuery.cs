using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetClubEventCountQuery : IRequest<ServiceResponse<int>>
    {
    }
}