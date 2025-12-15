using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Event.Command
{
    public class EventCheckInCommand : IRequest<ServiceResponse<string>>
    {
        public Guid EventCheckInId { get; set; }
    }
}
