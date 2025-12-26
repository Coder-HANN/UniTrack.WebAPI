using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class ClubEntityRepository : BaseEntityRepository<Club>, IClubRepository
    {
        public ClubEntityRepository(UniTrackDbContext context) : base(context)
        {
        }

        public Task<long> Get180DaysActiveClubsCountAsync()
        {
            var hundredEightyDaysAgo = DateTime.UtcNow.AddDays(-180);
            return dbSet.LongCountAsync(c => c.LastLoginDate != null && c.LastLoginDate >= hundredEightyDaysAgo);
        }

        public Task<long> Get30DaysActiveClubsCountAsync()
        {
            var thrityDaysAgo = DateTime.UtcNow.AddDays(-30);
            return dbSet.LongCountAsync(c => c.LastLoginDate != null && c.LastLoginDate >= thrityDaysAgo);
        }

        public Task<long> Get360DaysActiveClubsCountAsync()
        {
            var threeSixtyDaysAgo = DateTime.UtcNow.AddDays(-360);
            return dbSet.LongCountAsync(c => c.LastLoginDate != null && c.LastLoginDate >= threeSixtyDaysAgo);
        }

        public Task<long> Get90DaysActiveClubsCountAsync()
        {
            var ninetyDaysAgo = DateTime.UtcNow.AddDays(-90);
            return dbSet.LongCountAsync(c => c.LastLoginDate != null && c.LastLoginDate >= ninetyDaysAgo);
        }

        public Task<List<Club>> GetAllClubAsync(Expression<Func<Club, bool>> expression)
        {
            return dbSet.ToListAsync();
        }

        public Task<Club> GetByEmailAndVerifyAsync(string presidentEmail)
        {
           return dbSet.FirstOrDefaultAsync(c => c.PresidentMail == presidentEmail && c.IsVerified == true);
        }

        public Task<Club> GetByEmailAsync(string email)
        {
            return dbSet.FirstOrDefaultAsync(c => c.PresidentMail == email);
        }

        public Task<Club> GetByIdAsync(Guid Id)
        {
            return dbSet.FirstOrDefaultAsync(c => c.Id == Id);
        }

        public async Task<long> GetClubCountAsync()
        {
            return await context.Set<Club>().LongCountAsync();
        }

        public Task<long> GetClubFollowerCountAsync(Guid value)
        {
            return context.Set<UserClub>()
                .Where(cf => cf.ClubId == value)
                .LongCountAsync();
        }

        public async Task<List<Guid>> GetFilteredClubIdsAsync(List<int>? cityIds,List<Guid>? universityIds,List<Guid>? clubIds)
        {
            IQueryable<Club> query = context.Clubs
                .Where(c => !c.IsDeleted && c.IsVerified);

            if (cityIds != null && cityIds.Any())
            {
                query = query.Where(c => cityIds.Contains(c.CityId));
            }

            if (universityIds != null && universityIds.Any())
            {
                query = query.Where(c => universityIds.Contains(c.UniversityId));
            }

            if (clubIds != null && clubIds.Any())
            {
                query = query.Where(c => clubIds.Contains(c.Id));
            }

            return await query
                .Select(c => c.Id)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<Guid>> GetUserClubIdsAsync(Guid userId)
        {
            return await context.UserClubs
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.ClubId)
                .Distinct()
                .ToListAsync();
        }
    }
}
