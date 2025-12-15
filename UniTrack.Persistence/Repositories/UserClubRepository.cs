using Microsoft.EntityFrameworkCore;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class UserClubRepository : BaseEntityRepository<UserClub>, IUserClubRepository
    {
        public UserClubRepository(UniTrackDbContext context) : base(context)
        {
        }

        public Task<List<UserClub>> GetClubFollowersAsync(Guid clubId)
        {
          return dbSet.Where(c => c.ClubId == clubId)
                .Where(c => c.IsFollowing == true)
                .ToListAsync();
        }

        public Task<UserClub> GetClubFollowersByUserIdAsync(Guid value, Guid userDetailId)
        {
            return dbSet.FirstOrDefaultAsync(c => c.ClubId == value && c.UserId == userDetailId && c.IsFollowing == true);
		}

        public Task<int> GetFollowedClubCountAsync(Guid? userId)
        {
            return dbSet
                .Where(c => c.UserId == userId && c.IsFollowing == true)
                .CountAsync();
        }

        public Task<UserClub> GetUserIdInClubAsync(Guid clubId, Guid userId)
        {
            return dbSet.FirstOrDefaultAsync(c => c.ClubId == clubId && c.UserId == userId);
        }
    }
}
