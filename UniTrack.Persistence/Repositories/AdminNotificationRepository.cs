using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class AdminNotificationRepository : BaseEntityRepository<AdminNotification>, IAdminNotificationRepository
    {
        public AdminNotificationRepository(UniTrackDbContext context) : base(context)
        {
        }
    }
}
