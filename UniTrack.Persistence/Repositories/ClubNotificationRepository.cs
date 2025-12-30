using Google.Apis.Drive.v3.Data;
using Microsoft.EntityFrameworkCore;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class ClubNotificationRepository : BaseEntityRepository<ClubNotification>, IClubNotificationRepository
    {
        public ClubNotificationRepository(UniTrackDbContext context) : base(context)
        {
        }

        public async Task<ClubNotification> GetByClubAndNotificationIdAsync(Guid clubId, Guid notificationId)
        {
            return await context.ClubNotifications
               .AsNoTracking() // sadece okuma için (istersen kaldırabilirsin)
                   .FirstOrDefaultAsync(un =>
                   un.ClubId == clubId &&
                   un.NotificationId == notificationId);
        }

        public async Task<List<ClubNotification>> GetClubAllNotification(Guid clubId)
        {
            return await context.Set<ClubNotification>()
               .Where(n => n.ClubId == clubId)
               .OrderByDescending(n => n.CreatedDate)
               .ToListAsync();
        }
        public async Task<List<ClubNotification>> GetClubNotificationsAsync(Guid clubId, int take = 50)
        {
            return await context.Set<ClubNotification>()
                .Where(n => n.ClubId == clubId && !n.IsRead)
                .OrderByDescending(n => n.CreatedDate)
                .Take(take)
                .ToListAsync();
        }

        public async Task<bool> MarkAllAsReadAsync(Guid? clubId)
        {
            var affectedRows = await context.ClubNotifications
               .Where(n => n.ClubId == clubId && !n.IsRead)
               .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));

            return affectedRows > 0;
        }
    }
}
