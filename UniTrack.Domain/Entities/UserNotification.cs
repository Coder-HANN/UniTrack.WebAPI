namespace UniTrack.Domain.Entities
{
    public class UserNotification : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid NotificationId { get; set; }
        public bool IsRead { get; set; }
        public DateTimeOffset? ReadDate { get; set; }
        public Notification Notification { get; set; }
        public User User { get; set; }
    }
}
