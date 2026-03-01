using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.VerificationCode.Command
{
    public class ResetClubPasswordCommand : IRequest<ServiceResponse<string>>
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }
    }
}
