using MediatR;
using Microsoft.AspNetCore.Http;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Profile;

namespace UniTrack.Application.Feature.EventImage.Command
{
    public class UploadEventImageCommand : IRequest<ServiceResponse<UploadProfileImageResponseDTO>>
    {
        public IFormFile File { get; set; } = null!;
    }
}