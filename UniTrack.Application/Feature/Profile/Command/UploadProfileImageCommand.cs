using MediatR;
using Microsoft.AspNetCore.Http;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Profile;

namespace UniTrack.Application.Feature.Profile.Command
{
    public class UploadProfileImageCommand : IRequest<ServiceResponse<UploadProfileImageResponseDTO>>
    {
        public IFormFile File { get; set; } = null!;
    }
}