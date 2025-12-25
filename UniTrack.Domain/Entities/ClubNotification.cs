using UniTrack.Domain.Entities;

namespace UniTrack.Domain.Entities
{
    public class ClubNotification : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid ClubId { get; set; }
        public Guid NotificationId { get; set; }
        public bool IsRead { get; set; }
        public DateTimeOffset? ReadDate { get; set; }
        public Club Club { get; set; }
        public Notification Notification { get; set; }
    }
}