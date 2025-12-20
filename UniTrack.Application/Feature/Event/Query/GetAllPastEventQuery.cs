using MediatR;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetAllPastEventQuery : IRequest<ServiceResponse<IPagingExecutionResult<GetAllPastEventQueryResponseDTO>>>
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
