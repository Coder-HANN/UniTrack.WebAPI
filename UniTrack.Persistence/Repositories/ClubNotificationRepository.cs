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
    }
}
