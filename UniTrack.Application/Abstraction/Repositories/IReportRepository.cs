using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IReportRepository : IBaseEntityRepository<Report>
    {
        Task<Report> GetByIdAsync(Guid reportId);
        Task<bool> GetReportClubAsync(Guid userId, Guid clubId);
        Task<bool> GetReportEventAsync(Guid userId, Guid eventId);
    }
}
