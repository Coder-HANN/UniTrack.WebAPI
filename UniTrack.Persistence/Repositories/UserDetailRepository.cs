using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class UserDetailRepository : BaseEntityRepository<UserDetail>, IUserDetailRepository
    {
        public UserDetailRepository(UniTrackDbContext context) : base(context)
        {
        }

        public async Task<UserDetail?> GetByUserIdAsync(Guid userId)
        {
            return await context.UserDetails
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<UserDetail> GetUserForJoinAsync(Guid userId)
        {
            return await context.UserDetails
                .Include(ud => ud.User)
                .Include(ud => ud.City)
                .Include(ud => ud.University)
                .Include(ud => ud.Department)
                .FirstOrDefaultAsync(ud => ud.UserId == userId);
        }

        public async Task<List<UserDetail>> GetUsersByTargetAsync(List<int>? cityIds,List<Guid>? universityIds,List<int>? departmentIds,List<Guid>? clubIds)
        {
            var query = context.UserDetails
                .AsNoTracking()
                .Where(u =>
                    (cityIds == null || cityIds.Contains(u.CityId)) &&
                    (universityIds == null || universityIds.Contains(u.UniverstiyId)) &&
                    (departmentIds == null || departmentIds.Contains(u.DepartmentId))
                );

            if (clubIds != null && clubIds.Any())
            {
                query = query.Where(u =>
                    u.User.UserClubs.Any(uc => clubIds.Contains(uc.ClubId))
                );
            }

            return await query.ToListAsync();
        }
    }
}
