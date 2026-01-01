using UniTrack.Domain.Enums;

namespace UniTrack.Domain.Entities
{
    public class NotificationChannelType
    {
        public Guid NotificationId { get; set; }
        public Notification Notification { get; set; }
        public NotificationChannel Channel { get; set; }
    }
}
