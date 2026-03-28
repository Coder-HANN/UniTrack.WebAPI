using MediatR;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.VerificationCode.Command
{
    public class VerificationCommand : IRequest<ServiceResponse<string>>
    {
        public string Email { get; set; }
        public string VerificationCode { get; set; }
        public VerificationType VerificationType { get; set; }
    }

}
