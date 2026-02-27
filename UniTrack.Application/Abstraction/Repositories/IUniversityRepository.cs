using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IUniversityRepository : IBaseEntityRepository<University>
    {
        public Task<University> GetByIdAsync(Guid id);
        Task<IEnumerable<University>> GetAllAsync();
    }
}
