namespace UniTrack.Domain.Entities
{
    public class UserClub : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid ClubId { get; set; }
        public Club Club { get; set; }
        public bool IsFollowing { get; set; } = false;
        public bool IsNotification { get; set; } = false;
    }
}
