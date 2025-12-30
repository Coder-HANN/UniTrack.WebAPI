using UniTrack.Domain.Enums;

namespace UniTrack.Application.DTOs.Event
{
    public class FavoriteEventsResponseDTO
    {
        public string EventName { get; set; }
        public DateTimeOffset EventDate { get; set; }
        public string[]? EventImageUrl { get; set; }
        public string EventLocation { get; set; }
        public Time Time { get; set; }
        public long Qouta { get; set; }
        public long joinerCount { get; set; }
        public float Points { get; set; }
        public int PointsCount { get; set; }
    }
}
