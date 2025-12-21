using UniTrack.Domain.Enums;

namespace UniTrack.Application.DTOs.Event
{
    public class GetAllEventQueryResponseDTO
    {
        public byte[]? Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string Location { get; set; }
        public Guid ClubId { get; set; }
        public EventTag EventTag { get; set; }
        public Time Time { get; set; }
        public Status Status { get; set; }
        public long Quota { get; set; }
        public float Rate { get; set; }
    }
}
