namespace UniTrack.Application.DTOs.Profile
{
    public class GetClubProfileResponseDTO
    {
        public string Name { get; set; } = string.Empty;
        public string ContactMail { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
    }
}
