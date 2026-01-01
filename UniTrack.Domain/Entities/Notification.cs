using UniTrack.Domain.Enums;

namespace UniTrack.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid Id { get; set; }
        public string? LogoUrl { get; set; }
        public string? Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; } // Örn: "EVENT_CREATED", "EVENT_REMINDER", "EVENT_UPDATED"
        
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public Guid? RelatedEntityId { get; set; }
        public ICollection<TargetNotification> Targets { get; set; }
        public ICollection<UserNotification> UserNotifications { get; set; }
        public ICollection<ClubNotification> ClubNotifications { get; set; }
        public ICollection<NotificationChannelType> Channels { get; set; }
    }
}