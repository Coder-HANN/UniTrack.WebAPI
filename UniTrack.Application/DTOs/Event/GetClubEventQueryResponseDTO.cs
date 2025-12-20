using UniTrack.Domain.Enums;

namespace UniTrack.Application.DTOs.Event
{
    public class GetClubEventQueryResponseDTO
    {
        public byte[]? Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeOnly Clock {  get; set; }
        public string Location { get; set; }
        public Guid ClubId { get; set; }
        public EventTag EventTag { get; set; }
        public Time Time { get; set; }
        public Status Status { get; set; }
        public long Quota { get; set; }
    }
}
