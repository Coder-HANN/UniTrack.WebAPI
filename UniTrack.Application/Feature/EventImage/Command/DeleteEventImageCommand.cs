using MediatR;
using UniTrack.Application.Common;
namespace UniTrack.Application.Feature.EventImage
{
    public class DeleteEventImageCommand : IRequest<ServiceResponse<string>>
    {
        public Guid EventId { get; set; }
        public Guid ImageId { get; set; }
    }
}