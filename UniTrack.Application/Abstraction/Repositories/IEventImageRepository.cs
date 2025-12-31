using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IEventImageRepository : IBaseEntityRepository<EventImage>
    {
        Task AddRangeAsync(List<EventImage> images);
        Task UpdateRangeAsync(List<EventImage> images);
        Task<EventImage?> GetByIdAsync(Guid imageId);
        Task<List<EventImage>> GetByEventIdAsync(Guid eventId);

        Task DeleteByEventIdAsync(Guid eventId);

        Task<EventImage?> GetCoverImageAsync(Guid eventId);
        Task SetCoverImageAsync(Guid eventId, Guid imageId);

    }
}
