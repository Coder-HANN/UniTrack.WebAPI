using UniTrack.Domain.Enums;

namespace UniTrack.Application.DTOs.Comment
{
    public class GetAllJoinedEventForUserQueryResponseDTO
    {
        public byte[]? EventImage { get; set; }
        public string EventName { get; set; } 
        public string ShortDescription { get; set; }
        public string UniversityName { get; set; }
        public string Location { get; set; }
        public DateTimeOffset EventDate { get; set; }
        public string ClubName { get; set; }
        public DateTimeOffset JoinDate { get; set; }
        public EventTag EventTag { get; set; }
        public Time Time { get; set; }
        public bool? IsCheckIn { get; set; } 

    }
}
