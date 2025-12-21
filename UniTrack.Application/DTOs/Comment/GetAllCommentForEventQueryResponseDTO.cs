namespace UniTrack.Application.DTOs.Comment
{
    public class GetAllCommentForEventQueryResponseDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public int Point { get; set; }
        public string Description { get; set; }
    }
}
