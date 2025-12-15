using MediatR;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Abstraction.Services.PageBase;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetAllFeatureEventQuery : IRequest<ServiceResponse<IPagingExecutionResult<List<GetAllFeatureEventQueryResponseDTO>>>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public GetAllFeatureEventQuery(int Page, int PageSize)
        {
            this.Page = Page;
            this.PageSize = PageSize;
        }
        public GetAllFeatureEventQuery() { }
    }
}
