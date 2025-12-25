namespace UniTrack.Application.DTOs.Notification
{
    public class NotificationListResponse
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string? Logo { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
