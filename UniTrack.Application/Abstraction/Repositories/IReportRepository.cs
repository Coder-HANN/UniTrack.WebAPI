using UniTrack.Domain.Enums;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IReportRepository : IBaseEntityRepository<Report>
    {
        Task<bool> GetReportClubAsync(Guid userId, Guid clubId);
        Task<bool> GetReportEventAsync(Guid userId, Guid eventId);
    }
}
