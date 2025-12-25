using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class TargetNotificationRepository : BaseEntityRepository<TargetNotification>, ITargetNotificationRepository
    {
        public TargetNotificationRepository(UniTrackDbContext context) : base(context)
        {
        }
    }
}
