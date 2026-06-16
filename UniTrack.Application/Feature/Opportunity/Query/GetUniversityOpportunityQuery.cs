using MediatR;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Opportunity;

namespace UniTrack.Application.Feature.Opportunity.Query
{
    public class GetUniversityOpportunityQuery : IRequest<ServiceResponse<IPagingExecutionResult<GetAllOpportunityResponseDTO>>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}