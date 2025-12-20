using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface ILikeRepository : IBaseEntityRepository<Like>
    {
        public Task<int> GetLikesCountComment(Guid commentId);
    }
}
