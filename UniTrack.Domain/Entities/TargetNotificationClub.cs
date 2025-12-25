namespace UniTrack.Domain.Entities
{
    public class TargetNotificationClub
    {
        public Guid Id { get; set; }
        public Guid TargetNotificationId { get; set; }
        public Guid ClubId { get; set; }
        public TargetNotification TargetNotification { get; set; }
        public Club Club { get; set; }
    }
}
