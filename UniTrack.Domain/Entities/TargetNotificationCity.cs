namespace UniTrack.Domain.Entities
{
    public class TargetNotificationCity
    {
        public Guid Id { get; set; }
        public Guid TargetNotificationId { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
        public TargetNotification TargetNotification { get; set; }
    }

}
