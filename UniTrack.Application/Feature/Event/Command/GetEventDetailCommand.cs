using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Command
{
    public class GetEventDetailCommand : IRequest<ServiceResponse<GetEventDetailResponseDTO>>
    {
        public Guid EventId { get; set; }
        public GetEventDetailCommand(){}
    }
}
