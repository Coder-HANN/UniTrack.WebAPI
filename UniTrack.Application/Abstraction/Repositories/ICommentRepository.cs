using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface ICommentRepository : IBaseEntityRepository<Comment>
    {
        public Task<Comment> GetCommentIdAsync(Guid commentId);
        public Task<Comment> GetCommentByEventAndUserIdAsync(Guid commentId, Guid userId);
        public Task<Comment> GetCommentByClubAndUserIdAsync(Guid commentId, Guid userId);
        public Task<List<Comment>> GetAllCommentsByClubIdAsync(Guid clubId);
        public Task<List<Comment>> GetAllCommentByEventIdAsync(Guid eventId);
        public Task<List<Comment>> GetAllCommentsByUserIdAsync(Guid userId);
        public Task<double> GetClubAverageRatingAsync(Guid clubId);
        public Task<double> GetEventAverageRatingAsync(Guid eventId);
        Task<Dictionary<Guid, (float AverageRating, int Count)>> GetEventsRatingsSummaryAsync(List<Guid> eventIds);
        public Task<bool> IncrementLikeCountAsync(Guid commentId);
    }
}
