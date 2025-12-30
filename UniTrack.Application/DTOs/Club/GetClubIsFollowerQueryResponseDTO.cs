namespace UniTrack.Application.DTOs.Club
{
    public class GetClubIsFollowerQueryResponseDTO
    {
        public string? ImageUrl { get; set; } 
        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid UniversityId { get; set; }
        public string Department { get; set; }
    }
}
