using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IUserClubRepository : IBaseEntityRepository<UserClub>
    {
        Task<List<UserClub>> GetClubFollowersAsync(Guid clubId);
		Task<UserClub> GetClubFollowersByUserIdAsync(Guid value, Guid userId);
        Task<int> GetFollowedClubCountAsync(Guid? userId);
        Task<List<UserClub>> GetFollowedClubsByUserIdAsync(Guid value);
        Task<UserClub> GetUserIdInClubAsync(Guid clubId, Guid userId);

        // Kulübü takip eden ve bildirimi açık olan kullanıcıları çeker
        Task<List<Guid>> GetUsersWithNotificationOpenForClubAsync(Guid clubId);
        Task<List<UserClub>> SearchFollowersByNameAsync(Guid clubId, string name);
    }
}
