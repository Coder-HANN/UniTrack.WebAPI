using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class ReportRepository : BaseEntityRepository<Report>, IReportRepository
    {
        public ReportRepository(UniTrackDbContext context) : base(context)
        {
        }
    }
}
