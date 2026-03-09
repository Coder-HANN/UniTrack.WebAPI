// UniTrack.Persistence/Repositories/EventQuestionEntityRepository.cs
using Microsoft.EntityFrameworkCore;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class EventQuestionEntityRepository : BaseEntityRepository<EventQuestion>, IEventQuestionRepository
    {
        public EventQuestionEntityRepository(UniTrackDbContext context) : base(context)
        {
        }

        public async Task<List<EventQuestion>> GetByEventIdWithDetailsAsync(Guid eventId)
        {
            return await context.Set<EventQuestion>()
                .Include(q => q.User)
                    .ThenInclude(u => u.UserDetail)
                .Include(q => q.Answer)
                    .ThenInclude(a => a.Club)
                .Where(q => q.EventId == eventId)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasUserAskedQuestionAsync(Guid eventId, Guid userId)
        {
            return await context.Set<EventQuestion>()
                .AnyAsync(q => q.EventId == eventId && q.UserId == userId);
        }
    }
}