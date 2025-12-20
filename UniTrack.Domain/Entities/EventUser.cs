namespace UniTrack.Domain.Entities
{
    public class EventUser : BaseEntity
    {
        public Guid EventId { get; set; }
        public Event Event { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public bool IsJoined { get; set; } = false;
        public bool IsCheckedIn { get; set; } = false;
        public DateTimeOffset CheckedInAt { get; set; }
        public bool? IsJoinedForSponsor { get; set; } = false;
    }
}