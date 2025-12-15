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
    public class GetClubEventJoinQuery : IRequest<ServiceResponse<List<GetClubEventJoinQueryResponseDTO>>>
    {
        public GetClubEventJoinQuery()
        {
        }

        public Guid EventId { get; set; }
        public GetClubEventJoinQuery(Guid eventId)
        {
            EventId = eventId;
        }
    }
}
