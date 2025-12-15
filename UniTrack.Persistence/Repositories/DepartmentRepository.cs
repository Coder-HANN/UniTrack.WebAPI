using Microsoft.EntityFrameworkCore;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class DepartmentRepository : BaseEntityRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(UniTrackDbContext context) : base(context)
        {
        }

        public Task<Department> GetByIdAsync(int id, string name)
        {
            return dbSet.Where(d => d.Id == id && d.Name == name).FirstOrDefaultAsync();
        }

        public Task<Department> GetDepartmentByNameAsync(string name)
        {
            return dbSet.Where(d => d.Name == name).FirstOrDefaultAsync();
        }
    }
}
