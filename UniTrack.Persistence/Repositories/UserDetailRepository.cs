using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class UserDetailRepository : BaseEntityRepository<UserDetail>, IUserDetailRepository
    {
        public UserDetailRepository(UniTrackDbContext context) : base(context)
        {
        }
    }
}
