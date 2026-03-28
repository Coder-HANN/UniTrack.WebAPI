using MediatR;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.VerificationCode.Command
{
    public class ResendVerificationCommand : IRequest<ServiceResponse<string>>
    {
        public string Email { get; set; }
        public VerificationType VerificationType { get; set; } 
    }
}
