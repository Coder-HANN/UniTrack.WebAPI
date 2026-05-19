using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class NotificationRepository : BaseEntityRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(UniTrackDbContext context) : base(context) { }
    }
}