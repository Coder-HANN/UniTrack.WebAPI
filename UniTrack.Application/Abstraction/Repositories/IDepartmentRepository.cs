using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IDepartmentRepository : IBaseEntityRepository<Department>
    {
        public Task<Department> GetDepartmentByNameAsync(string name);
        public Task<Department> GetByIdAsync(int id,string name);

    }
}
