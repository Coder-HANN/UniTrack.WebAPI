using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.VerificationCode.Command
{
    public class ResendVerificationCommand : IRequest<ServiceResponse<string>>
    {
    }
}
