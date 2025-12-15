using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.University.Command
{
    public class CreateUniversityCommand : IRequest<ServiceResponse<string>>
    {
        public string Name { get; set; }
        public int CityId { get; set; }
    }
}
