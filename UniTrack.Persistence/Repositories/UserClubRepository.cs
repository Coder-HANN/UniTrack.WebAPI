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

        public async Task<List<UserClub>> GetFollowedClubsByUserIdAsync(Guid value)
        {
            return await dbSet
                .Where(c => c.UserId == value && c.IsFollowing == true)
                .Include(c => c.Club) // Kulüp bilgilerini de dahil et
                .ToListAsync();
        }

        public Task<UserClub> GetUserIdInClubAsync(Guid clubId, Guid userId)
        {
            return dbSet.FirstOrDefaultAsync(c => c.ClubId == clubId && c.UserId == userId);
        }

        public async Task<List<Guid>> GetUsersWithNotificationOpenForClubAsync(Guid clubId)
        {
            return await context.Set<UserClub>()
              .Where(uc => uc.ClubId == clubId && uc.IsNotification == true) // Sadece bildirimi açık olanlar
              .Select(uc => uc.UserId)
              .ToListAsync();
        }

        public async Task<List<UserClub>> SearchFollowersByNameAsync(Guid clubId, string name)
        {
            var lowerName = name.ToLower();
            return await context.UserClubs
                .Include(uc => uc.User.UserDetail)
                .Where(uc => uc.ClubId == clubId &&
                             (uc.User.UserDetail.Name.ToLower().Contains(lowerName) ||
                              uc.User.UserDetail.Surname.ToLower().Contains(lowerName)))
                .Take(10)
                .ToListAsync();
        }
    }
}
