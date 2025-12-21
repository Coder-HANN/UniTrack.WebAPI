using UniTrack.Domain.Enums;

namespace UniTrack.Domain.Entities
{
    public class Ban : BaseEntity
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool IsBanned { get; set; }
        public DateTimeOffset? LastDate { get; set; }
        public Role Role { get; set; }
        public Guid? UserId { get; set; }
        public User User { get; set; }
        public Guid? ClubId { get; set; }
        public Club Club { get; set; }
        public Guid? EventId { get; set; }
        public Event Event { get; set; }
    }
}
