// UniTrack.Application/Abstraction/Repositories/IEventQuestionRepository.cs
using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IEventQuestionRepository : IBaseEntityRepository<EventQuestion>
    {
        Task<List<EventQuestion>> GetByEventIdWithDetailsAsync(Guid eventId);
        Task<bool> HasUserAskedQuestionAsync(Guid eventId, Guid userId);
    }
}