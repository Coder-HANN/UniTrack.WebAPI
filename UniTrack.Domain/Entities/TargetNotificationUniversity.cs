namespace UniTrack.Domain.Entities
{
    public class TargetNotificationUniversity
    {
        public Guid Id { get; set; }
        public Guid TargetNotificationId { get; set; }
        public Guid UniversityId { get; set; }
        public TargetNotification TargetNotification { get; set; }
        public University University { get; set; }
    }
}
