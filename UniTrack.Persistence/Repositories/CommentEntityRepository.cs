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
                          .Include(c => c.User)
                              .ThenInclude(u => u.UserDetail)
                           .Include(c => c.Likes)
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

        public async Task<(double Average, int Count)> GetClubAverageRatingAsync(Guid clubId)
        {
            var allPoints = await context.Set<Event>()
                .Where(e => e.ClubId == clubId && e.Comments.Any())
                .SelectMany(e => e.Comments.Select(c => c.Point))
                .ToListAsync();

            if (!allPoints.Any())
                return (0, 0);

            return (allPoints.Average(), allPoints.Count);
        }


        public async Task<double> GetEventAverageRatingAsync(Guid eventId)
        {
            var averageRating = await context.Set<Comment>()
                                        .Where(c => c.EventId == eventId)
                                        .Select(c => (double?)c.Point)
                                        .AverageAsync();

            return averageRating ?? 0.0;
        }

        public Task<Comment> GetCommentByClubAndUserIdAsync(Guid commentId,Guid userId)
        {
            return dbSet.FirstOrDefaultAsync(c => c.Id == commentId && c.UserId== userId);
        }

        public Task<Comment> GetCommentByEventAndUserIdAsync(Guid EventId,Guid userId)
        {
            return  dbSet.FirstOrDefaultAsync(c => c.UserId == userId && c.EventId == EventId);
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

        public async Task<bool> IncrementLikeCountAsync(Guid commentId)
        {
             return await context.Comments
                .Where(c => c.Id == commentId)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(c => c.LikeCount, c => c.LikeCount + 1)
                ) > 0;
        }

        public async Task<int> CountAsync(Guid eventId)
        {
            return await context.Set<Comment>().CountAsync(c => c.EventId == eventId);
        }

        public async Task<bool> DecrementLikeCountAsync(Guid commentId)
        {
            var affected = await context.Comments
                .Where(c => c.Id == commentId)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(c => c.LikeCount, c => c.LikeCount - 1)
                );

            return affected > 0;
        }
    }
}
