// UniTrack.Application/Feature/EventLike/Command/UnlikeEventCommand.cs
using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Event.Command
{
    public class UnlikeEventCommand : IRequest<ServiceResponse<string>>
    {
        public Guid EventId { get; set; }
    }
}