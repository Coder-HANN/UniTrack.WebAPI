namespace UniTrack.Application.DTOs.Comment
{
    public class GetAllCommentByUserQueryResponseDTO
    {
        public Guid? CommentId { get; set; }
        public Guid? EventId { get; set; }
        public Guid? ClubId { get; set; }
        public string? CommentText { get; set; }
        public int Point {  get; set; }
    }
}
