using UniTrack.Domain.Enums;

namespace UniTrack.Application.DTOs.Event
{
    public class GetClubEventQueryResponseDTO
    {
        public string? CoverImageUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public TimeOnly StartTime {  get; set; }
        public TimeOnly EndTime { get; set; }
        public string Location { get; set; }
        public Guid ClubId { get; set; }
        public EventTag EventTag { get; set; }
        public Time Time { get; set; }
        public Status Status { get; set; }
        public long Quota { get; set; }
        public float Rate { get; set; }
        public Guid EventId { get; set; }
        public int JoinedCount { get; set; }
    }
}
