using Microsoft.EntityFrameworkCore;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class CommentEntityRepository : BaseEntityRepository<Comment>, ICommentRepository
    {
        public CommentEntityRepository(UniTrackDbContext context) : base(context)
        {
        }

        public async Task<List<Comment>> GetAllCommentByEventIdAsync(Guid eventId)
        {
            return await context.Comments
                          .Where(c => c.EventId == eventId)
                          .Include(c => c.User) // Comment'in User'ını yükle
                              .ThenInclude(u => u.UserDetail) // User'ın UserDetail'ını yükle
                          .ToListAsync();
        }

        public Task<List<Comment>> GetAllCommentsByClubIdAsync(Guid clubId)
        {
            return dbSet.Where(c => c.ClubId == clubId).ToListAsync();
        }


        public async Task<List<Comment>> GetAllCommentsByUserIdAsync(Guid userId)
        {
            return await context.Set<Comment>()
                            .Where(c => c.UserId == userId)
                            .Include(c => c.User)
                                .ThenInclude(u => u.UserDetail)
                            .ToListAsync();
        }

        public async Task<float> GetClubAverageRatingAsync(Guid clubId)
        {
            var eventAvarage= await context.Set<Event>()
                .Where(e => e.ClubId == clubId && e.Comments.Any())
                .Select(e => e.Comments.Average(c => c.Point))
                .ToListAsync();

            return (float)eventAvarage.Average();
        }


        public async Task<float> GetEventAverageRatingAsync(Guid eventId)
        {
            var averageRating = await context.Set<Comment>()
                                        .Where(c => c.EventId == eventId)
                                        .Select(c => (double?)c.Point)
                                        .AverageAsync();

            return (float)(averageRating ?? 0);
        }

        public Task<Comment> GetCommentByClubAndUserIdAsync(Guid commentId,Guid userId)
        {
            return dbSet.FirstOrDefaultAsync(c => c.Id == commentId && c.UserId== userId);
        }

        public Task<Comment> GetCommentByEventAndUserIdAsync(Guid commentId, Guid userId)
        {
            return  dbSet.FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);
        }

        public Task<Comment> GetCommentIdAsync(Guid commentId)
        {
            return dbSet.FirstOrDefaultAsync(c => c.Id == commentId);
        }

        public async Task<Dictionary<Guid, (float AverageRating, int Count)>> GetEventsRatingsSummaryAsync(List<Guid> eventIds)
        {
            var ratingSummaries = await context.Set<Comment>() // Context'teki Comment tablosunu kullan
                .Where(c => eventIds.Contains(c.EventId)) // Sadece ilgili etkinlikleri filtrele
                .GroupBy(c => c.EventId) // Etkinlik ID'sine göre grupla
                .Select(g => new
                {
                    EventId = g.Key,
                    // Ortalama puanı al ve isimlendir
                    AverageRating = g.Average(c => (float?)c.Point) ?? 0f,
                    // Yorum sayısını al ve isimlendir
                    Count = g.Count()
                })
                .ToDictionaryAsync(
                    x => x.EventId,
                    x => (x.AverageRating, x.Count) // Dictionary değeri olarak Tuple dönüyoruz
                );

            return ratingSummaries;
        }
    }
}
