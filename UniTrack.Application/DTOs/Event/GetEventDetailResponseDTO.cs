using UniTrack.Domain.Enums;

namespace UniTrack.Application.DTOs.Event
{
    public class GetEventDetailResponseDTO
    {
        public Guid Id { get; set; }
        public EventTag EventTag { get; set; }
        public Status Status { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public TimeOnly StartTime { get; set; }   // ← eklendi
        public TimeOnly EndTime { get; set; }
        public string Location { get; set; }
        public Guid ClubId { get; set; }
        public string ClubName { get; set; }
        public string ContectMail { get; set; }
        public long Quota { get; set; }
        public int JoinedCount { get; set; }
        public double Rate { get; set; }
        public string[]? ImageUrls { get; set; }
        public bool IsJoined { get; set; }
        public int CityId { get; set; }
        public Guid UniversityId { get; set; }
        public string GoogleSheetsUrl { get; set; }
        public bool IsLiked { get; set; }
        public int LikeCount { get; set; }
    }
}
