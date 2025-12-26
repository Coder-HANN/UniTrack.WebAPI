using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface ITargetNotificationCityRepository : IBaseEntityRepository<TargetNotificationCity>
    {
        Task AddRangeAsync(List<TargetNotificationCity> targetNotificationCities);
    }
}
