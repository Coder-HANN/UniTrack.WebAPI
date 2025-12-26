using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface ITargetNotificationUniversityRepository : IBaseEntityRepository<TargetNotificationUniversity>
    {
        Task AddRangeAsync(List<TargetNotificationUniversity> targetNotificationUniversities);
    }
}
