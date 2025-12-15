using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.University.Command
{
    public class DeleteUniversityCommand : IRequest<ServiceResponse<string>>
    {
        public Guid Id { get; set; }
    }
}
