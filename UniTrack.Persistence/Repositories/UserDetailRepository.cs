using Microsoft.EntityFrameworkCore;
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
