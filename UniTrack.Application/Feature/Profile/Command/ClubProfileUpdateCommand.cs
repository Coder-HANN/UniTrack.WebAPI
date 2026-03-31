using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Profile.Command
{
    public class ClubProfileUpdateCommand : IRequest<ServiceResponse<ClubProfileUpdateResponseDTO>>
    {
        public string? Name { get; set; }
        public Guid? UniversityId { get; set; }
        public string? President { get; set; }
        public string? PresidentMail { get; set; }
        public string? ContectEmail { get; set; }
        public string? Description { get; set; }
        public string? LongDescription { get; set; }
        public string? InstagramLink { get; set; }
        public string? TwitterLink { get; set; }
        public string? WebsiteLink { get; set; }
        public string? LinkedlnLink { get; set; }
        public string? TikTokLink { get; set; }
        public string? LogoUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public Tag? Tag { get; set; }
        public DateOnly? ClubCreatedDate { get; set; }
        public string? NowPassword { get; set; }
        public string? Password { get; set; }
    }
}
