using Microsoft.EntityFrameworkCore;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class EventUserRepository : BaseEntityRepository<EventUser>, IEventUserRepository
    {
        public EventUserRepository(UniTrackDbContext context) : base(context)
        {
        }

        public async Task<List<EventUser>> GetAllJoinedEventForUserAsync(Guid? userId)
        {
            return await dbSet
                .Where(eu => eu.UserId == userId && eu.IsJoined == true)
                .Include(eu => eu.Event)
                .Include(eu => eu.Event.Club)
                .Include(eu => eu.Event.Club.University)
                .Include(eu => eu.User.Comments)
                .ToListAsync();
        }

        public async Task<List<EventUser>> GetClubEventJoinsByClubIdAsync(Guid eventId)
        {
            return await dbSet.Where(eu => eu.EventId == eventId && eu.IsJoined == true)
                .Include(eu => eu.User)
                    .ThenInclude(u => u.UserDetail)
                        .ThenInclude(ud => ud.Department)
                .Include(eu => eu.User)
                    .ThenInclude(u => u.UserDetail)
                    .ThenInclude(ud => ud.University)
                .ToListAsync();
        }

        public async Task<EventUser> GetEventoinUserIdAsync(Guid eventId)
        {
            return await dbSet.FirstOrDefaultAsync(eu => eu.EventId == eventId);
                
        }

        public async Task<EventUser> GetEventUserCheckInAsync(Guid userId, Guid eventCheckInId)
        {
            return await dbSet.FirstOrDefaultAsync(eu => eu.IsCheckedIn == true && eu.UserId == userId && eu.Event.CheckInToken == eventCheckInId);
        }

        public async Task<int> GetJoinEventCountAsync(Guid? userId)
        {
            return await dbSet
                .Where(eu => eu.UserId == userId && eu.IsJoined == true && eu.IsCheckedIn == true)
                .Select(eu => eu.Event)
                .Distinct()
                .CountAsync();
        }

        public Task<int> GetTotalJoinerCountByClubIdAsync(Guid clubId)
        {
            return dbSet
                .Where(eu => eu.Event.ClubId == clubId && eu.IsJoined == true)
                .CountAsync();
        }
        public async Task<List<Guid>> GetUsersJoinedToEventAsync(Guid eventId)
        {
            return await context.Set<EventUser>()
                .Where(eu => eu.EventId == eventId && eu.IsJoined)
                .Select(eu => eu.UserId)
                .ToListAsync();
        }

    }
}
