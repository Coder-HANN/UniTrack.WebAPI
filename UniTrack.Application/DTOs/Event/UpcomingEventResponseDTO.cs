namespace UniTrack.Application.DTOs.Event
{
    public class UpcomingEventResponseDTO
    {
        public Guid EventId { get; set; }
        public string Title { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public string ClubName { get; set; }
        public bool IsDoping { get; set; }
        public string? ClubLogoUrl { get; set; }
    }
}