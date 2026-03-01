using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Command
{
    public class EventIsClubCountCommand : IRequest<ServiceResponse<EventIsClubCountResponseDTO>>
    {
        public Guid ClubId { get; set; }
    }
}
