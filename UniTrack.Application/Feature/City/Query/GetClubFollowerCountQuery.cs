using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.City.Query
{
    public class GetClubFollowerCountQuery : IRequest<ServiceResponse<long>>
    {
        public Guid ClubId { get; set; }
        public GetClubFollowerCountQuery(Guid clubId)
        {
            ClubId = clubId;
        }
        public GetClubFollowerCountQuery() { }
    }
}
