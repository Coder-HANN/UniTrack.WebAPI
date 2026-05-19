using MediatR;
using Microsoft.AspNetCore.Http;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Profile;

namespace UniTrack.Application.Feature.OpportunityImage.Command
{
    public class UploadOpportunityImageCommand : IRequest<ServiceResponse<UploadProfileImageResponseDTO>>
    {
        public IFormFile File { get; set; }
    }
}