using UniTrack.Domain.Enums;

namespace UniTrack.Application.DTOs.Ban
{
    public class GetBanedClubOrUserQueryResponseDTO
    {
        public Guid Id { get; set; }
        public Role Role { get; set; }
        public Guid? UserId { get; set; }
        public Guid? ClubId { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? LastDate { get; set; }
    }
}
