using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class TargetNotificationCityRepository : BaseEntityRepository<TargetNotificationCity>, ITargetNotificationCityRepository
    {
        public TargetNotificationCityRepository(UniTrackDbContext context) : base(context)
        {
        }
    }
}
