namespace UniTrack.Domain.Entities
{
    public class ClubTeam : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public Guid ClubId { get; set; }
        public Club Club { get; set; }
        public User User { get; set; }

    }
}
