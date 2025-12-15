using MediatR;
using MediatR.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
