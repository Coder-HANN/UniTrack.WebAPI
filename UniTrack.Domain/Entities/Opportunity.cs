using UniTrack.Domain.Enums;

namespace UniTrack.Domain.Entities
{
    public class Opportunity : BaseEntity
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string? ImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Link { get; set; }
        public Category Category { get; set; }
        public OpportunityScope Scope { get; set; }
        public DateTimeOffset LastDate { get; set; }
        public string? Code { get; set; }
        public ICollection<OpportunityUser> OpportunityUsers { get; set; }
        public ICollection<OpportunityUniversity> OpportunityUniversities { get; set; }
    }
}