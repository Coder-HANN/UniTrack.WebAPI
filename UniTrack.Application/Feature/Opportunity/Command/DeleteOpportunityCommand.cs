using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Opportunity.Command
{
    public class DeleteOpportunityCommand : IRequest<ServiceResponse<string>>
    {
        public Guid Id { get; set; }
    }
}
