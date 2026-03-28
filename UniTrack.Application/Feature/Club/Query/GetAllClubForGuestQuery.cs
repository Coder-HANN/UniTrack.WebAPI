using MediatR;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Club;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetAllClubForGuestQuery : IRequest<ServiceResponse<IPagingExecutionResult<GetAllClubForGuestQueryResponseDTO>>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
