namespace UniTrack.Application.DTOs.Club
{
    public class GetClubDetailResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? LongDescription { get; set; }
        public string? LogoUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? InstagramLink { get; set; }
        public string? TwitterLink { get; set; }
        public string? WebsiteLink { get; set; }
        public string? TikTokLink { get; set; }
        public string? LinkedlnLink { get; set; }
        public string ContactEmail { get; set; }
        public string President { get; set; }
        public string PresidentMail { get; set; }
        public int Tag { get; set; }
        public long FollowerCount { get; set; }
        public int EventCount { get; set; }
        public string UniversityName { get; set; }
        public string CityName { get; set; }
        public DateOnly ClubCreatedDate { get; set; }
        public bool IsFollowed { get; set; }
        public bool IsNotificationOn { get; set; }
    }
}