using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Command
{
    public class UserJoinToEventCommand : IRequest<ServiceResponse<UserJoinToEventResponseDTO>>
    {
        public Guid EventId { get; set; }
    }
}
