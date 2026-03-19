using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetClubUpcomingEventsQuery : IRequest<ServiceResponse<List<UpcomingEventResponseDTO>>>
    {
    }
}
