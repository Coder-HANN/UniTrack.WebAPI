using Microsoft.EntityFrameworkCore;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class BanRepository : BaseEntityRepository<Ban>, IBanRepository
    {
        public BanRepository(UniTrackDbContext context) : base(context)
        {}

        public async Task<Ban> GetActiveBanForClubAsync(Guid clubId)
        {
            return await context.Bans
                .Include(b => b.Club)
                .Where(b => b.ClubId == clubId && b.Role == Role.Club && b.IsBanned && b.IsDeleted == false)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Ban>> GetBannedClubInUniversityAsync(Guid? UniversityId)
        {
            return await context.Bans
                .Include(b => b.Club)
                .Where(b => b.Role == Role.Club && b.IsBanned && b.Club.UniversityId == UniversityId && b.IsDeleted == false)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ban>> GetBannedUserInUniversityAsync(Guid? UniversityId)
        {
            return await context.Bans
                .Include(b=>b.User)
                    .ThenInclude(u=>u.UserDetail)
                .Include(b=>b.User.UserClubs)

                .Where(b => b.Role == Role.User && b.IsBanned && b.User.UserDetail.UniverstiyId == UniversityId && b.IsDeleted == false)
                .ToListAsync();
        }

        public async Task<Ban> GetByIdAsync(Guid banId)
        {
            return await dbSet.FirstOrDefaultAsync(b => b.Id == banId);
        }

        public async Task<bool> IsBannedAsync(Guid id, Role role)
        {
            return await dbSet.AnyAsync(b =>
            (role == Role.User && b.UserId == id && b.IsBanned) ||
            (role == Role.Club && b.ClubId == id && b.IsBanned) && 
            (b.IsDeleted == false));
        }

        public async Task<bool> LiftBanIfExpiredAsync(Guid id, Role role)
        {
            var ban = await context.Bans
                .Where(b => b.Role == role && b.IsBanned)
                .Where(b =>
                    (role == Role.User && b.UserId == id) || (role == Role.Club && b.ClubId == id)).FirstOrDefaultAsync();

            if (ban == null || !ban.LastDate.HasValue || ban.LastDate > DateTime.UtcNow)
                return false;

            ban.IsBanned = true;
            ban.LastDate = null;
            ban.IsDeleted = false;

            await context.SaveChangesAsync();
            return true;
        }
    }
}
