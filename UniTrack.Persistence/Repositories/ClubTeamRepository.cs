using Microsoft.EntityFrameworkCore;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class ClubTeamRepository : BaseEntityRepository<ClubTeam>, IClubTeamRepository
    {
        public ClubTeamRepository(UniTrackDbContext context) : base(context)
        {
        }

        public Task<ClubTeam> GetClubTeamId(Guid clubTeamId)
        {
            return context.ClubTeams.FirstOrDefaultAsync(c => c.Id == clubTeamId);
        }

        public Task<List<ClubTeam>> GetClubTeamsByClubIdAsync(Guid clubId)
        {
            return dbSet.Where(c => c.ClubId == clubId)
                .Include(c => c.User)
                    .ThenInclude(u => u.UserDetail)
                .ToListAsync();
        }
    }
}