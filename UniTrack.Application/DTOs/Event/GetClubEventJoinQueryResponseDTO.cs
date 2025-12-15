namespace UniTrack.Application.DTOs.Event
{
    public class GetClubEventJoinQueryResponseDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Department { get; set; }
        public Guid UniversityId { get; set; }

    }
}
