using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;

namespace UniTrack.Persistence.Repositories
{
    public interface ITargetNotificationClubRepository : IBaseEntityRepository<TargetNotificationClub>
    {
        Task AddRangeAsync(List<TargetNotificationClub> targetNotificationClubs);
    }
}