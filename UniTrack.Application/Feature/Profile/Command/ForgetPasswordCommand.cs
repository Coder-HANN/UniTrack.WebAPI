using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Profile.Command
{
    public class ForgetPasswordCommand : IRequest<ServiceResponse<string>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
