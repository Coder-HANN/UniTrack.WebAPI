using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Notification;

namespace UniTrack.Application.Feature.Notification.Query
{
    public class PreviewTargetNotificationQuery: IRequest<ServiceResponse<PreviewTargetNotificationResponse>>
    {
        public List<int>? CityIds { get; set; }
        public List<Guid>? UniversityIds { get; set; }
        public List<int>? DepartmentIds { get; set; }
        public List<Guid>? ClubIds { get; set; }
    }
}
