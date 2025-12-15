namespace UniTrack.Application.DTOs.Club
{
    public class GetClubIsFollowerQueryResponseDTO
    {
        public byte? Image { get; set; } // pp
        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid UniversityId { get; set; }
        public string Department { get; set; }
    }
}
