using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Opportunity;

namespace UniTrack.Application.Feature.Opportunity.Command
{
    public class ViewedCodeForOpportunityCommand : IRequest<ServiceResponse<ViewedCodeForOpportunityResponseDTO>>
    {
        public Guid OpportunityId { get; set; }
    }
}