using MediatR;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Club;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetAllClubQuery : IRequest<ServiceResponse<IPagingExecutionResult<GetAllClubQueryResponseDTO>>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public GetAllClubQuery(int Page,int PageSize) {
            this.Page = Page;
            this.PageSize = PageSize;
        }
        public GetAllClubQuery() { }
    }
}
