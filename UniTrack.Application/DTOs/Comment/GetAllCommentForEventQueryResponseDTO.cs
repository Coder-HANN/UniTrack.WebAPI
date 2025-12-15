namespace UniTrack.Application.DTOs.Comment
{
    public class GetAllCommentForEventQueryResponseDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Point { get; set; }
        public string Description { get; set; }
    }
}
