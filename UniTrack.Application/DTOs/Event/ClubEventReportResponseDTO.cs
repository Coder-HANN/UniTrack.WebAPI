namespace UniTrack.Application.DTOs.Event
{
    public class ClubEventReportResponseDTO
    {
        public Guid EventId { get; set; }
        public string Title { get; set; }
        public int JoinedCount { get; set; }
        public int Quota { get; set; }
        public double FillRate { get; set; }
    }
}