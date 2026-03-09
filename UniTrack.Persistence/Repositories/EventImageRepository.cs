using Microsoft.EntityFrameworkCore;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class EventImageRepository : BaseEntityRepository<EventImage>, IEventImageRepository
    {
        public EventImageRepository(UniTrackDbContext context) : base(context)
        {
        }

        public async Task AddRangeAsync(List<EventImage> images)
        {
            if (images == null || !images.Any())
                return;

            await context.EventImages.AddRangeAsync(images);
            await context.SaveChangesAsync();
        }

        public async Task<List<EventImage>> GetByEventIdAsync(Guid eventId)
        {
            return await context.EventImages
                .Where(x => x.EventId == eventId)
                .OrderBy(x => x.Order)
                .ToListAsync();
        }

        public async Task DeleteByEventIdAsync(Guid eventId)
        {
            var images = await context.EventImages
                .Where(x => x.EventId == eventId)
                .ToListAsync();

            if (!images.Any())
                return;

            context.EventImages.RemoveRange(images);
            await context.SaveChangesAsync();
        }

        public async Task<EventImage?> GetCoverImageAsync(Guid eventId)
        {
            return await context.EventImages
                .FirstOrDefaultAsync(x => x.EventId == eventId && x.IsCover);
        }

        public async Task SetCoverImageAsync(Guid eventId, Guid imageId)
        {
            var images = await context.EventImages
                .Where(x => x.EventId == eventId)
                .ToListAsync();

            foreach (var image in images)
            {
                image.IsCover = image.Id == imageId;
                image.Order = image.IsCover ? 0 : image.Order;
            }

            await context.SaveChangesAsync();
        }


        public async Task UpdateRangeAsync(List<EventImage> images)
        {
            context.EventImages.UpdateRange(images);
            await context.SaveChangesAsync();
        }

        public async Task<EventImage?> GetByIdAsync(Guid imageId)
        {
            return await context.EventImages.FirstOrDefaultAsync(x => x.Id == imageId);
        }

        public async Task<List<EventImage>> GetByEventIdsAsync(List<Guid> eventIds)
        {
            return await context.EventImages
               .Where(x => eventIds.Contains(x.EventId))
               .OrderBy(x => x.Order)
               .ToListAsync();
        }
    }
}
