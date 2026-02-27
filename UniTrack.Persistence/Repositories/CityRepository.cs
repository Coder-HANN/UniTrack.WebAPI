using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class CityRepository : BaseEntityRepository<City>, ICityRepository
    {
        public CityRepository(UniTrackDbContext context) : base(context)
        {

        }
    }
}
