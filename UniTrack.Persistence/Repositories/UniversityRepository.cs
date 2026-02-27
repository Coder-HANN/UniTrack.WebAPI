using Microsoft.EntityFrameworkCore;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class UniversityRepository : BaseEntityRepository<University>, IUniversityRepository
    {
        public UniversityRepository(UniTrackDbContext context) : base(context)
        {
        }

        public Task<University> GetByIdAsync(Guid id)
        {
            return dbSet.Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<University>> GetAllAsync()
        {
            return await context.Universities
            .AsNoTracking()
            .OrderBy(u => u.Name)
            .ToListAsync();
        }
    }
}
