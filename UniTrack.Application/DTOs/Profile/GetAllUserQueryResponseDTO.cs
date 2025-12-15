namespace UniTrack.Application.DTOs.Profile
{
    public class GetAllUserQueryResponseDTO
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public Guid UniversityId { get; set; }
        public int DepartmentId { get; set; }
    }
}
