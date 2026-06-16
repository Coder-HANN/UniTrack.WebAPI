using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IOpportunityRepository : IBaseEntityRepository<Opportunity>
    {
        Task<Opportunity> GetByIdAsync(Guid opportunityId);
        Task<List<Opportunity>> GetAllOpportunityAsync(OpportunityScope scope); // Global için
        Task<List<Opportunity>> GetOpportunitiesByUniversityAsync(Guid universityId); // Üniversiteye özel
    }
}
