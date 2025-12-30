using MediatR;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class PushTargetNotificationCommand: IRequest<ServiceResponse<bool>>
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }

        public int? CityId { get; set; }
        public Guid? UniversityId { get; set; }
        public int? DepartmentId { get; set; }
        public List<Guid> ClubIds { get; set; } = new();

        public Guid? RelatedEntityId { get; set; }
    }
}
