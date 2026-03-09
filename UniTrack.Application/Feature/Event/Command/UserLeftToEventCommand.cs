using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Event.Command
{
    public class UserLeftToEventCommand : IRequest<ServiceResponse<string>>
    {
        public Guid EventId { get; set; }
    }
}
