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

        public async Task<bool> IsBannedAsync(Guid id, Role role)
        {
            return await dbSet.AnyAsync(b =>
            (role == Role.User && b.UserId == id && b.IsBanned) ||
            (role == Role.Club && b.ClubId == id && b.IsBanned));
        }

        public async Task<bool> LiftBanIfExpiredAsync(Guid id, Role role)
        {
            var ban = await context.Bans
                .Where(b => b.Role == role && b.IsBanned)
                .Where(b =>
                    (role == Role.User && b.UserId == id) || (role == Role.Club && b.ClubId == id)).FirstOrDefaultAsync();

            if (ban == null || !ban.LastDate.HasValue || ban.LastDate > DateTime.UtcNow)
                return false;

            ban.IsBanned = false;
            ban.LastDate = null;

            await context.SaveChangesAsync();
            return true;
        }
    }
}
