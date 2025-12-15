using UniTrack.Domain.Enums;

namespace UniTrack.Application.DTOs.Profile
{
    public class UserProfileUpdateResponseDTO
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public Guid? UniverstiyId { get; set; }
        public int? DepartmentId { get; set; }
        public Gender? Gender { get; set; }
        public DateOnly? BirthDate { get; set; }
    }
}
