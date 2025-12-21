using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetClubEventQuery : IRequest<ServiceResponse<IPagingExecutionResult<GetClubEventQueryResponseDTO>>>
    {
        public GetClubEventQuery() { }
        public Guid ClubId { get; set; }
        public int Page { get; set; } 
        public int PageSize { get; set; }
        public GetClubEventQuery(Guid clubId, int page, int pageSize)
        {
            ClubId = clubId;
            Page = page;
            PageSize = pageSize;
        }
    }
}
