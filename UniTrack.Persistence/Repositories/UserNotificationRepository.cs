using Microsoft.EntityFrameworkCore;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class UserNotificationRepository : BaseEntityRepository<UserNotification>, IUserNotificationRepository
    {
        public UserNotificationRepository(UniTrackDbContext context) : base(context)
        {
        }

        public async Task<List<UserNotification>> GetUserAllNotification(Guid userId)
        {
            return await context.Set<UserNotification>()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<UserNotification>> GetUserNotificationsAsync(Guid userId)
        {
            return await context.UserNotifications
            .Include(x => x.Notification)
            .Where(x => x.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task AddRangeAsync(List<UserNotification> userNotifications)
        {
            await context.Set<UserNotification>().AddRangeAsync(userNotifications);
            await context.SaveChangesAsync();
        }


        public async Task<UserNotification?> GetByUserAndNotificationIdAsync(Guid userId, Guid notificationId)
        {
            return await context.UserNotifications
                .AsNoTracking() // sadece okuma için (istersen kaldırabilirsin)
                    .FirstOrDefaultAsync(un =>
                    un.UserId == userId &&
                    un.NotificationId == notificationId);
        }    
    }
}
