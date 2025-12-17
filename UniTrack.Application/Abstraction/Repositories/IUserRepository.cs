using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IUserRepository : IBaseEntityRepository<User>
    {
        Task<long> CountAsync();
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync (Guid Id);
        Task<long> For90DaysCountAsync();
        Task <long>Get180DaysActiveUsersCountAsync();
        Task <long>Get360DaysActiveUsersCountAsync();
        Task<long> GetUserCountAsync();
    }
}
