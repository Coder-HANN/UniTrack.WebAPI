namespace UniTrack.Domain.Entities
{
    public class TargetNotification : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid NotificationId { get; set; }
        public Notification Notification { get; set; }
        public ICollection<TargetNotificationCity> City { get; set; }
        public ICollection<TargetNotificationClub> Clubs { get; set; }
        public ICollection<TargetNotificationDepartment> Departments { get; set; }
        public ICollection<TargetNotificationUniversity> Universities { get; set; }
    }
}
