using Google.Apis.Drive.v3.Data;
using Microsoft.EntityFrameworkCore;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{

    public class NotificationRepository : BaseEntityRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(UniTrackDbContext context) : base(context) {}

        public async Task<List<Notification>> GetUserAllNotification(Guid userId)
        {
            return await context.Set<Notification>()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(Guid userId, int take = 50)
        {
            return await context.Set<Notification>()
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetClubNotificationsAsync(Guid clubId, int take = 50)
        {
            return await context.Set<Notification>()
                .Where(n => n.ClubId == clubId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<Guid>> GetUsersJoinedToEventAsync(Guid eventId)
        {
            return await context.Set<EventUser>()
                .Where(eu => eu.EventId == eventId && eu.IsJoined)
                .Select(eu => eu.UserId)
                .ToListAsync();
        }

        public async Task<List<Guid>> GetUsersWithNotificationOpenForClubAsync(Guid clubId)
        {
          return await context.Set<UserClub>()
            .Where(uc => uc.ClubId == clubId && uc.IsNotification) // Sadece bildirimi açık olanlar
            .Select(uc => uc.UserId)
            .ToListAsync();
        }

        public async Task<List<Notification>> GetClubAllNotification(Guid clubId)
        {
            return await context.Set<Notification>()
               .Where(n => n.ClubId == clubId)
               .OrderByDescending(n => n.CreatedAt)
               .ToListAsync();
        }
    }
}
