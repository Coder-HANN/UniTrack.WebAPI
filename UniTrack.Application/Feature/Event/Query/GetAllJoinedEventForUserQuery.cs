using MediatR;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Abstraction.Services.PageBase;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Comment;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetAllJoinedEventForUserQuery : IRequest<ServiceResponse<IPagingExecutionResult<GetAllJoinedEventForUserQueryResponseDTO>>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public GetAllJoinedEventForUserQuery(int Page, int PageSize) 
        {
            this.Page = Page;
            this.PageSize = PageSize;
        }
        public GetAllJoinedEventForUserQuery() { }
    }
}
