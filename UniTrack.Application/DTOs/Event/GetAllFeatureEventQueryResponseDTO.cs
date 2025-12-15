using UniTrack.Domain.Enums;

namespace UniTrack.Application.DTOs.Event
{
    public class GetAllFeatureEventQueryResponseDTO
    {
        public byte[]? Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public Guid ClubId { get; set; }
        public Tag Tags { get; set; }
        public Time Time { get; set; }
        public Status Status { get; set; }
        public long Quota { get; set; }
        public float Rate { get; set; }
    }
}
