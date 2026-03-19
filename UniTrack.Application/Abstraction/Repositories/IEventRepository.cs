using System.Linq.Expressions;
using UniTrack.Application.DTOs.Event;
using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IEventRepository : IBaseEntityRepository<Event>
    {
         public Task<Event> GetByIdAsync(Guid Id);
         Task<List<Event>> GetAllClubEventAsync(Expression<Func<Event, bool>> expression);
         Task <int> GetClubEventCountAsync(Guid clubId);
         Task<List<Event>> GetFeatureEventsAsync();
         Task<List<Event>> GetPastEventsAsync();
         Task <long> GetCountAsync();
         Task<long> GetAllEventFeatureCountAsync();
         Task<long> GetAllPastEventCountAsync();
         public Task<List<Event>> GetTopThreeFavoriteEventsByClubIdAsync(Guid clubId);
         public Task<List<Event>> GetAllEventAsync(Expression<Func<Event, bool>> expression);
         public Task<Event> GetEventByIdAndClubIdAsync(Guid eventId, Guid clubId);
        public Task<bool> CountaddedAsync(Guid EventId);
        public Task<long> GetAllClubEventJoinerCountAsync(Guid? clubId);
        public Task<Club>GetClubNameByIdAsync(Guid clubId);
        public Task<Event> GetEventDetailByIdAsync(Guid eventId);
        Task<Event> GetEventIdAsync(Guid eventId);
        Task<List<Event>> GetUpcomingDopingEventsAsync(DateTimeOffset now, int take);

        Task<List<Event>> GetUpcomingJoinedEventsAsync(Guid userId, DateTimeOffset now, int take, HashSet<Guid> excludeIds);

        Task<List<Event>> GetUpcomingGeneralEventsAsync(DateTimeOffset now, int take, HashSet<Guid> excludeIds);
        Task<List<MonthlyParticipationResponseDTO>> GetMonthlyParticipationAsync(Guid userId, DateTime startDate);
        Task<List<Event>> GetUpcomingEventsByClubIdAsync(Guid clubId, DateTimeOffset now, int maxCount);
        Task<int> GetCompletedEventCountByClubIdAsync(Guid clubId, DateTimeOffset now);
    }
}
