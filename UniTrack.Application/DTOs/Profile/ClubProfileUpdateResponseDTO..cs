using UniTrack.Domain.Enums;

namespace UniTrack.Application.DTOs.Profile
{
    public class ClubProfileUpdateResponseDTO
    {
        public string? Name { get; set; }
        public Guid UniversityId { get; set; }
        public string? President { get; set; }
        public string? PresidentMail { get; set; }
        public string? ContectEmail { get; set; }
        public string? Description { get; set; }
        public string? LongDescription { get; set; }
        public string? InstagramLink { get; set; }
        public string? TwitterLink { get; set; }
        public string? WebsiteLink { get; set; }
        public string? LinkedlnLink { get; set; }
        public byte? Logo { get; set; }
        public byte? CoverImage { get; set; }
        public Tag? Tag { get; set; }
        public DateOnly? ClubCreatedDate { get; set; }
   
    }
}
