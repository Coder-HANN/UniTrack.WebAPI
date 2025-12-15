using UniTrack.Domain.Enums;

namespace UniTrack.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public NotificationType Type { get; set; } // Örn: "EVENT_CREATED", "EVENT_REMINDER", "EVENT_UPDATED"
        public Guid? RelatedEntityId { get; set; } // Etkinlik veya Kulüp ID'si
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public User User { get; set; }
    }
}