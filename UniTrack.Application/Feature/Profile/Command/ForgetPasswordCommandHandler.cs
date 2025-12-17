using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Profile.Command
{
    public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, ServiceResponse<string>>
    {
        public Task<ServiceResponse<string>> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
// TO DO: Implement forget password functionality