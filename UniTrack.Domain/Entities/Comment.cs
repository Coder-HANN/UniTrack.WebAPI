namespace UniTrack.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public Guid Id { get; set; }
        public int Point { get; set; }
        public string? Description { get; set; }
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public Guid ClubId { get; set; }
        public int LikeCount { get; set; } = 0;
        public User User { get; set; }
        public Event Event { get; set; }
        public Club Club { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}
