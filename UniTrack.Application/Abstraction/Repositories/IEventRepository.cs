using System.Linq.Expressions;
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
    }
}
