using MediatR;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Opportunity.Command
{
    public class CreateOpportunityCommand : IRequest<ServiceResponse<string>>
    {
        public string CompanyName { get; set; }
        public string? ImageUrl { get; set; }   // Upload endpoint'ten alınan URL
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Link { get; set; }
        public Category Category { get; set; }
        public DateTimeOffset LastDate { get; set; }
        public string? Code { get; set; }
    }
}