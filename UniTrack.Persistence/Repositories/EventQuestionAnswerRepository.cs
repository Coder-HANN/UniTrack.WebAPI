using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class EventQuestionAnswerRepository : BaseEntityRepository<EventQuestion>
    {
        public EventQuestionAnswerRepository(UniTrackDbContext context) : base(context)
        {
        }
        
    }
}
