using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class TargetNotificationUniversityRepository : BaseEntityRepository<TargetNotificationUniversity>, ITargetNotificationUniversityRepository
    {
        public TargetNotificationUniversityRepository(UniTrackDbContext context) : base(context)
        {
        }
    }
}