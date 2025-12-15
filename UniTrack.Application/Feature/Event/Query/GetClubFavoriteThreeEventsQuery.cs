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
    public class GetClubFavoriteThreeEventsQuery : IRequest<ServiceResponse<List<FavoriteEventsResponseDTO>>>
    {
        public Guid ClubId { get; set; }
        public GetClubFavoriteThreeEventsQuery() { }
    }
}
