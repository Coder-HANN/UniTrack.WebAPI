namespace UniTrack.Domain.Entities
{
    public class TargetNotificationDepartment
    {
        public Guid Id { get; set; }
        public Guid TargetNotificationId { get; set; }
        public int DepartmentId { get; set; }
        public TargetNotification TargetNotification { get; set; }
        public Department Department { get; set; }
    }

}
