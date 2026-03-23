namespace UniTrack.Application.DTOs.Club
{
    public class ClubFollowerSearchResponseDTO
    {
        public Guid UserDetailId { get; set; }
        public string FullName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string Mail { get; set; }
    }
}