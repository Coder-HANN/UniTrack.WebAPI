using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.EventImage.Command
{
    public class ChangeEventCoverImageCommand : IRequest<ServiceResponse<string>>
    {
        public Guid EventId { get; set; }
        public Guid ImageId { get; set; }
    }

}
