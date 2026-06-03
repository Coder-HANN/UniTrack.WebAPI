// CalendarEventResponseDTO.cs
namespace UniTrack.Application.DTOs.Event
{
    public class CalendarEventResponseDTO
    {
        public Guid EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string ClubName { get; set; } = string.Empty;
        public string? ClubLogoUrl { get; set; }
        public bool? IsDoping { get; set; }
        public string? Location { get; set; }  
        public string? Time { get; set; }
    }
}