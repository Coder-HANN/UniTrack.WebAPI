using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Command
{
    public class DeleteEventCommand : IRequest<ServiceResponse<DeleteEventResponseDTO>>
    {
        public Guid EventId { get; set; }
    }
}
