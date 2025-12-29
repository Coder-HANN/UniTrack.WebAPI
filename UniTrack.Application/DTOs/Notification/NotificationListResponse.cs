namespace UniTrack.Application.DTOs.Notification
{
    public class NotificationListResponse
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string? LogoUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public Guid? RelatedEntityId { get; set; }
    }
}
