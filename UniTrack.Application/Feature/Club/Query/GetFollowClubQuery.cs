using MediatR;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Club;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetFollowClubQuery : IRequest<ServiceResponse<IPagingExecutionResult<GetFollowClubQueryResponseDTO>>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public GetFollowClubQuery(int Page, int PageSize)
        {
            this.Page = Page;
            this.PageSize = PageSize;
        }
        public GetFollowClubQuery() { }
    }
}
