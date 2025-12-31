namespace UniTrack.Domain.Entities
{
    public class AdminNotification : BaseEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
