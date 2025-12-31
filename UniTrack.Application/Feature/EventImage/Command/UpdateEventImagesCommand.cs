using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.EventImage.Command
{
    public class UpdateEventImagesCommand: IRequest<ServiceResponse<string>>
    {
        public Guid EventId { get; set; }
        public List<string> ImageUrls { get; set; }
    }

}
