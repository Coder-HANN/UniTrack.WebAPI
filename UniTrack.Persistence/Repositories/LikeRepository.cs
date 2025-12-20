using Microsoft.EntityFrameworkCore;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class LikeRepository : BaseEntityRepository<Like>, ILikeRepository
    {
        public LikeRepository(UniTrackDbContext context) : base(context)
        {
        }

        public Task<int> GetLikesCountComment(Guid commentId)
        {
            return context.Likes.CountAsync(l => l.CommentId == commentId);

        }
    }
}
