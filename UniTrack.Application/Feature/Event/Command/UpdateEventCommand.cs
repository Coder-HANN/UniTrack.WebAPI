using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Event.Command
{
    public class UpdateEventCommand : IRequest<ServiceResponse<UpdateEventResponseDTO>>
    {
        public Guid Id { get; set; }
        public List<string> ImageUrls { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public EventTag EventTag { get; set; }
        public int Quota { get; set; }
        public string Location { get; set; }
        public Status Status { get; set; }
    }
}
