namespace UniTrack.Domain.Entities
{
    public class OpportunityUser : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid OpportunityId { get; set; }
        public Opportunity Opportunity { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public bool Viewed { get; set; } = false;
    }
}