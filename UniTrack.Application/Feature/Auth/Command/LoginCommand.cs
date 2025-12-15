using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Auth;

namespace UniTrack.Application.Feature.Auth.Command
{
    public class LoginCommand : IRequest<ServiceResponse<LoginResponseDTO>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
