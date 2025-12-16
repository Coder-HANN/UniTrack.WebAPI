using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IBanRepository : BaseEntityRepository<Ban>
    {
        Task<Ban> GetByIdAsync(Guid banId);
        Task<bool> IsBannedAsync(Guid id, Role role);
        Task<bool> LiftBanIfExpiredAsync(Guid id, Role role);
    }
}
