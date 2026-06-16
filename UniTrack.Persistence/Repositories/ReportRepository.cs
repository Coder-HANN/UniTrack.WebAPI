using Microsoft.EntityFrameworkCore;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class ReportRepository : BaseEntityRepository<Domain.Entities.Report>, IReportRepository
    {
        public ReportRepository(UniTrackDbContext context) : base(context)
        {
        }

        public async  Task<Report> GetByIdAsync(Guid reportId)
        {
            return await context.Reports
                .Where(x => x.Id == reportId)
                .FirstOrDefaultAsync();
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

        async Task<IEnumerable<Report>> IReportRepository.GetReportForAdminAsync(Guid? universityId)
        {
            return await context.Reports
                .Include(r => r.User)
                    .ThenInclude(u => u.UserDetail)
                .Include(r => r.Club)
                .Include(r => r.Event)
                .Where(r => r.Event.UniversityId == universityId || r.Club.UniversityId == universityId  && r.IsDeleted == false)
                .ToListAsync();
        }
    }
}
