using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Auth;

namespace UniTrack.Application.Feature.Auth.Command
{
    public class AdminRegisterCommand : IRequest<ServiceResponse<AdminRegisterResponseDTO>>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid UniversityId { get; set; }
    }
}
