namespace UniTrack.Application.DTOs.Profile
{
    public class AdminProfileUpdateResponseDTO
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public Guid? UniverstiyId { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool? IsNotified { get; set; }
        public string? Password { get; set; }
    }
}
