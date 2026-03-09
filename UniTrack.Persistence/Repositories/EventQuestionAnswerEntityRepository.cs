// UniTrack.Persistence/Repositories/EventQuestionAnswerEntityRepository.cs
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class EventQuestionAnswerEntityRepository : BaseEntityRepository<EventQuestionAnswer>, IEventQuestionAnswerRepository
    {
        public EventQuestionAnswerEntityRepository(UniTrackDbContext context) : base(context)
        {
        }
    }
}