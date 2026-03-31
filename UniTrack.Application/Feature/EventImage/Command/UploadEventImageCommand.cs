using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Profile;

namespace UniTrack.Application.Feature.EventImage.Command
{
    public class UploadEventImageCommand : IRequest<ServiceResponse<UploadProfileImageResponseDTO>>
    {
        public string Base64 { get; set; }
    }
}