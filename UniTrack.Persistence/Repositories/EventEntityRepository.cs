using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Feature.Event.Models;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class EventEntityRepository : BaseEntityRepository<Event>, IEventRepository
    {
        public EventEntityRepository(UniTrackDbContext context) : base(context)
        {
        }

        public async Task<bool> CountaddedAsync(Guid EventId)
        {
            var joinerCount = await context.Events
                .Where(c => c.Id == EventId)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(c => c.Joiner, c => c.Joiner + 1)
                );

            return joinerCount > 0;
        }

        public Task<List<Event>> GetAllClubEventAsync(Expression<Func<Event, bool>> expression)
        {
            return dbSet.ToListAsync();
        }

        public async Task<long> GetAllClubEventJoinerCountAsync(Guid? clubId)
        {
            return await dbSet.Where(e => e.ClubId == clubId)
                .SumAsync(e => e.Joiner);
        }

        public async Task<List<Event>> GetAllEventAsync(Expression<Func<Event, bool>> expression)
        {
            return await dbSet.Include(e => e.Images).Where(expression).ToListAsync();
        }

        public Task<long> GetAllEventFeatureCountAsync()
        {
            return context.Set<Event>()
                .Where(e => e.EndDate >= DateTime.Today)
                .LongCountAsync();
        }

        public Task<long> GetAllPastEventCountAsync()
        {
            return context.Set<Event>()
                .Where(e => e.EndDate < DateTime.Today)
                .LongCountAsync();
        }

        public async Task<Event> GetByIdAsync(Guid Id)
        {
            return await dbSet.FirstOrDefaultAsync(e => e.Id == Id);
        }

        public async Task<int> GetClubEventCountAsync(Guid clubId)
        {
            return await context.Set<Event>()
                .Where(e => e.ClubId == clubId) // Sadece bu kulübün etkinlikleri
                .CountAsync();
        }

        public async Task<long> GetCountAsync()
        {
            return await context.Set<Event>().LongCountAsync();
        }

        public Task<Event> GetEventByIdAndClubIdAsync(Guid eventId, Guid clubId)
        {
            return context.Set<Event>()
                .FirstOrDefaultAsync(e => e.Id == eventId && e.ClubId == clubId);
        }

        public async Task<List<Event>> GetFeatureEventsAsync()
        {
            return await context.Set<Event>()
                .Where(e => e.EndDate >= DateTime.Today && e.IsDeleted == false)
                .ToListAsync();
        }

        public async Task<List<Event>> GetPastEventsAsync()
        {
            return await context.Set<Event>()
                .Where(e => e.EndDate < DateTime.Today && e.IsDeleted == false)
                .ToListAsync();
        }

        public async Task<List<Event>> GetTopThreeFavoriteEventsByClubIdAsync(Guid clubId)
        {
            return await context.Set<Event>()
                .Where(e => e.ClubId == clubId)
                .OrderByDescending(e => e.Joiner)
                .Take(3)
                .ToListAsync();
        }

    }
}
