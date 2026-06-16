namespace UniTrack.Domain.Entities
{
    public class OpportunityUniversity
    {
        public Guid Id { get; set; }
        public Guid OpportunityId { get; set; }
        public Opportunity Opportunity { get; set; }
        public Guid UniversityId { get; set; }
        public University University { get; set; }
    }
}