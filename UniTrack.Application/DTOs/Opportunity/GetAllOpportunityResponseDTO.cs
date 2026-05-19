namespace UniTrack.Application.DTOs.Opportunity
{
    public class GetAllOpportunityResponseDTO
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string? ImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Link { get; set; }
        public string Category { get; set; }
        public DateTimeOffset LastDate { get; set; }
        public string? Code { get; set; }
        public bool Viewed { get; set; }
    }
}