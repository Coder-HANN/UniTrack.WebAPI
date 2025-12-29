using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface ITargetNotificationRepository : IBaseEntityRepository<TargetNotification>
    {
        Task<List<TargetNotification>> GetMatchingNotificationsAsync(int? cityId, Guid? universityId, int? departmentId, List<Guid> clubIds);
        Task<List<TargetNotification>> GetMatchingNotificationsForClubAsync( int? cityId, Guid? universityId);
    }
}
