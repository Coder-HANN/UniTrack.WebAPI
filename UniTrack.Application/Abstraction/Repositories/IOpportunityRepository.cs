using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IOpportunityRepository : IBaseEntityRepository<Opportunity>
    {
        Task<Opportunity> GetByIdAsync(Guid opportunityId);
        Task<List<Opportunity>> GetAllOpportunityAsync();
    }
}
