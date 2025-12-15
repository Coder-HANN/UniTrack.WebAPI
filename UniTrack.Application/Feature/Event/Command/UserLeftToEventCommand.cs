using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Event.Command
{
    public class UserLeftToEventCommand : IRequest<ServiceResponse<string>>
    {
        public Guid EventId { get; set; }
    }
}
