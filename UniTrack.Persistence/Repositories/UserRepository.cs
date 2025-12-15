using Microsoft.EntityFrameworkCore;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class UserRepository : BaseEntityRepository<User>, IUserRepository
    {
        public UserRepository(UniTrackDbContext context) : base(context)
        {
        }

        public async Task<long> CountAsync()
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            return await dbSet.CountAsync(u =>
                u.LastLoginDate != null && 
                u.LastLoginDate >= thirtyDaysAgo
            );
        }

        public Task<long> For90DaysCountAsync()
        {
            var ninetyDaysAgo = DateTime.UtcNow.AddDays(-90);

            return dbSet.LongCountAsync(u =>
                u.LastLoginDate != null &&
                u.LastLoginDate >= ninetyDaysAgo
            );
        }

        public Task<long> Get180DaysActiveUsersCountAsync()
        {
            var oneEightyDaysAgo = DateTime.UtcNow.AddDays(-180);
            return dbSet.LongCountAsync(u =>
                u.LastLoginDate != null &&
                u.LastLoginDate >= oneEightyDaysAgo
            );
        }

        public Task<long> Get360DaysActiveUsersCountAsync()
        {
            var threeSixtyDaysAgo = DateTime.UtcNow.AddDays(-360);

            return dbSet.LongCountAsync(u =>
                u.LastLoginDate != null &&
                u.LastLoginDate >= threeSixtyDaysAgo
            );
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await context.Users.
                Include(u => u.UserDetail)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<User> GetByIdAsync(Guid Id)
        {
            return dbSet.FirstOrDefaultAsync(u => u.Id == Id);
        }

        public Task<long> GetUserCountAsync()
        {
            return dbSet.LongCountAsync();
        }
    }
}
