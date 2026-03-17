using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.DTOs.Event;
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
            return dbSet
                .Include(e=> e.EventUsers)
                .ToListAsync();
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

        public Task<Club> GetClubNameByIdAsync(Guid clubId)
        {
            return context.Set<Club>()
                .Where(c => c.Id == clubId)
                .Select(c => new Club { Id = c.Id, Name = c.Name }) // Sadece Id ve Name alanlarını seç
                .FirstOrDefaultAsync();
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

        public Task<Event> GetEventDetailByIdAsync(Guid eventId)
        {
            return context.Set<Event>()
                .Include(e => e.EventUsers)
                .Include(e => e.Club)
                .FirstOrDefaultAsync(e => e.Id == eventId);
        }

        public async Task<Event> GetEventIdAsync(Guid eventId)
        {
            return await context.Set<Event>()
                .Include(e => e.Club)
                .FirstOrDefaultAsync(e => e.Id == eventId);
        }

        public async Task<List<Event>> GetFeatureEventsAsync()
        {
            return await context.Set<Event>()
                .Include(e => e.EventUsers)
                .Where(e => e.EndDate >= DateTime.Today && e.IsDeleted == false)
                .ToListAsync();
        }

        public async Task<List<Event>> GetPastEventsAsync()
        {
            return await context.Set<Event>()
                .Include(e=> e.EventUsers)
                .Include(e => e.Club)
                    .ThenInclude(e => e.UserClubs)
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
        // Repository Implementasyonu
        public async Task<List<Event>> GetUpcomingDopingEventsAsync(DateTimeOffset now, int take)
        {
            return await context.Events
                .Include(e => e.Club)
                .Where(e =>
                    e.IsActived &&
                    e.IsDoping &&
                    e.StartDate > now)
                .OrderBy(e => e.StartDate)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<Event>> GetUpcomingJoinedEventsAsync(Guid userId, DateTimeOffset now, int take, HashSet<Guid> excludeIds)
        {
            return await context.Events
                .Include(e => e.Club)
                .Where(e =>
                    e.IsActived &&
                    e.StartDate > now &&
                    !excludeIds.Contains(e.Id) &&
                    e.EventUsers.Any(eu => eu.UserId == userId))
                .OrderBy(e => e.StartDate)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<Event>> GetUpcomingGeneralEventsAsync(DateTimeOffset now, int take, HashSet<Guid> excludeIds)
        {
            return await context.Events
                .Include(e => e.Club)
                .Where(e =>
                    e.IsActived &&
                    e.StartDate > now &&
                    !excludeIds.Contains(e.Id))
                .OrderBy(e => e.StartDate)
                .Take(take)
                .ToListAsync();
        }
        // Repository implementasyonu
        public async Task<List<MonthlyParticipationResponseDTO>> GetMonthlyParticipationAsync(Guid userId, DateTime startDate)
        {
            return await context.EventUsers
                .Include(eu => eu.Event)
                .Where(eu =>
                    eu.UserId == userId &&
                    eu.Event.IsActived &&
                    eu.Event.StartDate >= startDate)
                .GroupBy(eu => new { eu.Event.StartDate.Month, eu.Event.StartDate.Year })
                .Select(g => new MonthlyParticipationResponseDTO
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Count = g.Count()
                })
                .ToListAsync();
        }


    }
}
