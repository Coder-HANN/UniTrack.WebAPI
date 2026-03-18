using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Profile;

namespace UniTrack.Application.Feature.Profile.Command
{
    public class UploadProfileImageCommand : IRequest<ServiceResponse<UploadProfileImageResponseDTO>>
    {
        public string Base64 { get; set; } = string.Empty;
    }
}