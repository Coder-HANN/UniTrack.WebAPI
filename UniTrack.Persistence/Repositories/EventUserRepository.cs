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
                .ToListAsync();
        }

        public async Task<List<EventUser>> GetClubEventJoinsByClubIdAsync(Guid eventId)
        {
            return await dbSet.Where(eu => eu.EventId == eventId && eu.IsJoined == true).ToListAsync();
        }

        public async Task<EventUser> GetEventoinUserIdAsync(Guid eventId)
        {
            return await dbSet.FirstOrDefaultAsync(eu => eu.EventId == eventId);
                
        }

        public async Task<EventUser> GetEventUserCheckInAsync(Guid userId, Guid eventCheckInId)
        {
            return await dbSet.FirstOrDefaultAsync(eu => eu.IsCheckedIn == true && eu.UserId == userId && eu.Event.CheckInToken == eventCheckInId);
        }

        public Task<int> GetTotalJoinerCountByClubIdAsync(Guid clubId)
        {
            return dbSet
                .Where(eu => eu.Event.ClubId == clubId && eu.IsJoined == true)
                .CountAsync();
        }

    }
}
