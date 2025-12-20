namespace UniTrack.Domain.Entities
{
    public class Like : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid CommentId { get; set; }
        public Guid? ClubId { get; set; }
        public User User { get; set; }
        public Comment Comment { get; set; }
        public Club Club { get; set; }
    }
}
