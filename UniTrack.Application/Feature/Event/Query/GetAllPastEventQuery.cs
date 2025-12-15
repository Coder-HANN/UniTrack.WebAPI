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
    public class GetAllPastEventQuery : IRequest<ServiceResponse<IPagingExecutionResult<List<GetAllPastEventQueryResponseDTO>>>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public GetAllPastEventQuery(int Page, int PageSize)
        {
            this.Page = Page;
            this.PageSize = PageSize;
        }
        public GetAllPastEventQuery() { }
    }
}
