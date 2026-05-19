using Microsoft.EntityFrameworkCore;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class OpportunityRepository : BaseEntityRepository<Opportunity>, IOpportunityRepository
    {
        public OpportunityRepository(UniTrackDbContext context) : base(context)
        {
        }
        public async Task<List<Opportunity>> GetAllOpportunityAsync()
        {
            return await context.Opportunitys
                .Include(o => o.OpportunityUsers)
                .Where(o => o.IsDeleted == false)
                .ToListAsync(); // Veritabanı işlemini asenkron olarak tetikler
        }

        public async Task<Opportunity> GetByIdAsync(Guid opportunityId)
        {
            return await context.Opportunitys
                .Include(o =>o.OpportunityUsers)
                .FirstOrDefaultAsync(o => o.Id == opportunityId);
        }
    }
}
