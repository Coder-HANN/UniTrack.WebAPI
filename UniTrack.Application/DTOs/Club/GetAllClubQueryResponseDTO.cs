using UniTrack.Domain.Enums;

namespace UniTrack.Application.DTOs.Club
{
    public class GetAllClubQueryResponseDTO
    {
        public Guid Id { get; set; }
        public string? LogoUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string President { get; set; }
        public string ContactMail { get; set; }
        public long Followers { get; set; }
        public Tag Tag { get; set; }
    }
}
