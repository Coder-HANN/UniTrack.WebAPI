using UniTrack.Domain.Enums;

namespace UniTrack.Application.DTOs.Event
{
    public class GetAllFeatureEventQueryResponseDTO
    {
        public Guid EventId { get; set; }
        public int CityId { get; set; }
        public Guid UniversityId { get; set; }
        public string[]? CoverImageUrls { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public string Location { get; set; }
        public Guid ClubId { get; set; }
        public string ClubName { get; set; }
        public EventTag EventTag { get; set; }
        public Time Time { get; set; }
        public Status Status { get; set; }
        public long Quota { get; set; }
        public float Rate { get; set; }
        public bool IsJoin { get; set; }
    }
}
