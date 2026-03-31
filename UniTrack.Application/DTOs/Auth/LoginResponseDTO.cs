namespace UniTrack.Application.DTOs.Auth
{
    public class LoginResponseDTO
    {
        public DateTime Expiration { get; set; }
        public string Role { get; set; }
        public string? UserId { get; set; }
        public string? ClubId { get; set; }
        public string? FullName { get; set; }
        public string Email { get; set; }
        public string? UniversityId { get; set; }
        public int? CityId { get; set; }
        public int? DepartmentId { get; set; }
        public int? Gender { get; set; }
    }
}
