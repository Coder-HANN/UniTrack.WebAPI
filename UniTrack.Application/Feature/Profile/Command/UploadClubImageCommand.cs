using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Profile;

namespace UniTrack.Application.Feature.Profile.Command
{
    public class UploadClubImageCommand : IRequest<ServiceResponse<UploadProfileImageResponseDTO>>
    {
        public string Base64 { get; set; }
        public string ImageType { get; set; } // "logo" veya "cover"
    }
}