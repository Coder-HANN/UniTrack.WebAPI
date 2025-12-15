using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IUserClubRepository : BaseEntityRepository<UserClub>
    {
        Task<List<UserClub>> GetClubFollowersAsync(Guid clubId);
		Task<UserClub> GetClubFollowersByUserIdAsync(Guid value, Guid userDetailId);
        Task<int> GetFollowedClubCountAsync(Guid? userId);
        Task<UserClub> GetUserIdInClubAsync(Guid clubId, Guid userId);
    }
}
