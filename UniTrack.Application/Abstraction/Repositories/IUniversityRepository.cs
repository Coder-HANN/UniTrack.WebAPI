using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IUniversityRepository : BaseEntityRepository<University>
    {
        public Task<University> GetByIdAsync(Guid id);
    }
}
