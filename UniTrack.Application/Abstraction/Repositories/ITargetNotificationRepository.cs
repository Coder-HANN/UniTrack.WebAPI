using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface ITargetNotificationRepository : IBaseEntityRepository<TargetNotification>
    {
        Task<List<Notification>> GetMatchingNotificationsAsync(int? cityId, Guid? universityId, int? departmentId, List<Guid> clubIds);
    }
}
