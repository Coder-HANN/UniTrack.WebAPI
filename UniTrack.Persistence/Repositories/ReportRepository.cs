using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Enums;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class ReportRepository : BaseEntityRepository<Report>, IReportRepository
    {
        public ReportRepository(UniTrackDbContext context) : base(context)
        {
        }

        public async Task<bool> GetReportClubAsync(Guid userId, Guid clubId)
        {
            return await context.Reports.AnyAsync(r =>
                r.ReporterUserId == userId &&
                r.ClubId == clubId &&
                r.TargetType == ReportTargetType.Event &&
                r.Status == ReportStatus.Pending
            );
        }

        public async Task<bool> GetReportEventAsync(Guid userId, Guid eventId)
        {
            return await context.Reports.AnyAsync(r =>
                r.ReporterUserId == userId &&
                r.EventId == eventId &&
                r.TargetType == ReportTargetType.Event &&
                r.Status == ReportStatus.Pending
            );
        }

    }
}
