using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.ActiveDetail.Query
{
    public class GetActiveUsersFor360DaysQuery : IRequest<ServiceResponse<long>>
    {
        public GetActiveUsersFor360DaysQuery() { }
    }
}
