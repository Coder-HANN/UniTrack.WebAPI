using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetClubEventQuery : IRequest<ServiceResponse<List<GetClubEventQueryResponseDTO>>>
    {
        public GetClubEventQuery() { }
        public Guid ClubId { get; set; }
        public GetClubEventQuery(Guid clubId)
        {
            ClubId = clubId;
        }
    }
}
