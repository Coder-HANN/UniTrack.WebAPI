using System.Linq.Expressions;
using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IClubRepository : IBaseEntityRepository<Club>
    {
        Task<List<Club>> GetAllClubAsync(Expression<Func<Club,bool>> expression);
        Task<Club> GetByEmailAsync(string email);
        Task<Club> GetByIdAsync(Guid Id);
        Task <long>GetClubCountAsync();
        Task<long> Get30DaysActiveClubsCountAsync();
        Task<long> Get90DaysActiveClubsCountAsync();
        Task<long> Get180DaysActiveClubsCountAsync();
        Task<long> Get360DaysActiveClubsCountAsync();
        Task<long> GetClubFollowerCountAsync(Guid value);
        Task <Club>GetByEmailAndVerifyAsync(string presidentEmail);
        Task<List<Guid>> GetFilteredClubIdsAsync(List<int>? cityIds, List<Guid>? universityIds, List<Guid>? clubIds);
    }
}
