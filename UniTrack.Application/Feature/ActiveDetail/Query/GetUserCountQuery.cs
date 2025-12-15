using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniTrack.Application.Feature.ActiveDetail.Query
{
    public class GetUserCountQuery : IRequest<long>
    {
        public GetUserCountQuery() { }
    }
}
