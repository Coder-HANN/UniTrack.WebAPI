using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class TargetNotificationClubRepository : BaseEntityRepository<TargetNotificationClub>, ITargetNotificationClubRepository
    {
        public TargetNotificationClubRepository(UniTrackDbContext context) : base(context)
        {
        }

        public Task AddRangeAsync(List<TargetNotificationClub> targetNotificationClubs)
        {
            throw new NotImplementedException();
        }
    }
}
