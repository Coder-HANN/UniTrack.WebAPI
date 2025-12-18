using MediatR;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetAllEventQuery : IRequest<ServiceResponse<IPagingExecutionResult<GetAllEventQueryResponseDTO>>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public GetAllEventQuery(int Page , int PageSize) {
            this.Page = Page;
            this.PageSize = PageSize;
        }
        public GetAllEventQuery() { }

    }
}
