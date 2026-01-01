using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IUserDetailRepository : IBaseEntityRepository<UserDetail>
    {
        Task<List<UserDetail>> GetUsersByTargetAsync(List<int>? cityIds,List<Guid>? universityIds,List<int>? departmentIds,List<Guid>? clubIds);

    }
}
