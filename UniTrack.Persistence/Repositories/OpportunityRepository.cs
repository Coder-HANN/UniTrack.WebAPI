using Microsoft.EntityFrameworkCore;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class OpportunityRepository : BaseEntityRepository<Opportunity>, IOpportunityRepository
    {
        public OpportunityRepository(UniTrackDbContext context) : base(context)
        {
        }
        public async Task<List<Opportunity>> GetAllOpportunityAsync(OpportunityScope opportunityScope)
        {
            return await context.Opportunitys
                .Include(o => o.OpportunityUsers)
                .Where(o => o.IsDeleted == false && o.Scope == opportunityScope)
                .ToListAsync(); // Veritabanı işlemini asenkron olarak tetikler
        }

        public async Task<Opportunity> GetByIdAsync(Guid opportunityId)
        {
            return await context.Opportunitys
                .Include(o =>o.OpportunityUsers)
                .Include(o => o.OpportunityUniversities)
                .FirstOrDefaultAsync(o => o.Id == opportunityId);
        }

        public async Task<List<Opportunity>> GetOpportunitiesByUniversityAsync(Guid universityId)
        {
            return await context.Opportunitys
                .Include(o => o.OpportunityUsers)
                .Include(o => o.OpportunityUniversities)
                .Where(o => o.IsDeleted == false && o.OpportunityUniversities.Any(ou => ou.UniversityId == universityId))
                .ToListAsync();
        }
    }
}
