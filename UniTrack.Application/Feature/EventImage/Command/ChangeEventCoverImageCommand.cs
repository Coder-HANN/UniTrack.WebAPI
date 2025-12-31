using MediatR;

namespace UniTrack.Application.Feature.EventImage.Command
{
    public class ChangeEventCoverImageCommand : IRequest<string>
    {
        public Guid EventId { get; set; }
        public Guid ImageId { get; set; }
    }

}
