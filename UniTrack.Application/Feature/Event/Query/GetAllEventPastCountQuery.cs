using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetAllEventPastCountQuery : IRequest<ServiceResponse<long>>
    {
        public GetAllEventPastCountQuery() { }
    }
}
