using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.VerificationCode.Command
{
    public class VerificationCommand : IRequest<ServiceResponse<string>>
    {
        public string Email { get; set; }
        public string VerificationCode { get; set; }
    }

}
