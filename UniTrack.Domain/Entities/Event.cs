using UniTrack.Domain.Enums;

namespace UniTrack.Domain.Entities
{
    public class Event : BaseEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset StartDate { get; set; }  
        public DateTimeOffset EndDate { get; set; }
        public byte[]? Image { get; set; }
        public long Quota { get; set; }
        public long Joiner { get; set; }
        public EventTag EventTag { get; set; }
        public string Location { get; set; }
        public TimeOnly Clock { get; set; }  
        public Status Status { get; set; }  
        public Guid ClubId { get; set; }
        public Time Time { get; set; }
        public bool IsActived { get; set; } = true;
        public Club Club { get; set; }  
        public int CityId { get; set; }
        public Guid UniversityId { get; set; }
        public ICollection<EventUser> EventUsers { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public Ban Ban { get; set; }
        public City City { get; set; }
        public University University { get; set; }
        public string? SheetsId { get; set; }
        public Guid? CheckInToken { get; set; }
        public string? QrCodeUrl  { get; set; }
    }
}
