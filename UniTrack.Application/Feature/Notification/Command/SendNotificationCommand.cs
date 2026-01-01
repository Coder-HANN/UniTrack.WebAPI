using MediatR;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class SendNotificationCommand : IRequest<ServiceResponse<string>>
    {
        public string? Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public Guid? RelatedEntityId { get; set; }

        // 🔽 Filtreler
        public List<int>? CityIds { get; set; }
        public List<Guid>? UniversityIds { get; set; }
        public List<int>? DepartmentIds { get; set; }
        public List<Guid>? ClubIds { get; set; }
        public List<NotificationChannel> Channels { get; set; }

    }

}
