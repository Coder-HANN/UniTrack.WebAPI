namespace UniTrack.Application.DTOs.Event
{
    public class MonthlyParticipationResponseDTO
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; }
        public int Count { get; set; }
    }
}
