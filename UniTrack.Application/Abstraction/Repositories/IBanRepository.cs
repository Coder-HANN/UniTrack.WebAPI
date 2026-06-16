using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IBanRepository : IBaseEntityRepository<Ban>
    {
        Task <Ban>GetActiveBanForClubAsync(Guid clubId);
        Task<IEnumerable<Ban>> GetBannedClubInUniversityAsync(Guid? UniversityId);
        Task<IEnumerable<Ban>> GetBannedUserInUniversityAsync(Guid? UniversityId);
        Task<Ban> GetByIdAsync(Guid banId);
        Task<bool> IsBannedAsync(Guid id, Role role);
        Task<bool> LiftBanIfExpiredAsync(Guid id, Role role);
    }
}
