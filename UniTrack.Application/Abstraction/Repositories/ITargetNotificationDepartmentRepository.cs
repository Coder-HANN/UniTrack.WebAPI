using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface ITargetNotificationDepartmentRepository : IBaseEntityRepository<TargetNotificationDepartment>
    {
        Task AddRangeAsync(List<TargetNotificationDepartment> targetNotificationDepartments);
    }
}
