using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class TargetNotificationDepartmentRepository : BaseEntityRepository<TargetNotificationDepartment>, ITargetNotificationDepartmentRepository
    {
        public TargetNotificationDepartmentRepository(UniTrackDbContext context) : base(context)
        {
        }
    }
}
