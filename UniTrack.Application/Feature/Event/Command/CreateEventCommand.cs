using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Event.Command
{
    public class CreateEventCommand : IRequest<ServiceResponse<CreateEventResponseDTO>>
    {
        public List<string>? ImageUrls { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeOnly Clock { get; set; }
        public Tag Tag { get; set; }
        public long Quota { get; set; }
        public string Location { get; set; }
        public Status Status { get; set; }
        public Guid ClubId { get; set; }
        public int CityId { get; set; }
        public Guid UniversityId { get; set; }
    }
}
