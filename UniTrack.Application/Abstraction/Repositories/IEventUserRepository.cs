using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IEventUserRepository : IBaseEntityRepository<EventUser>
    {
        public Task<List<EventUser>> GetAllJoinedEventForUserAsync(Guid? userId);
        public Task<List<EventUser>> GetClubEventJoinsByClubIdAsync(Guid eventId);
        public Task<EventUser> GetEventoinUserIdAsync(Guid eventId);
        public Task<EventUser> GetEventUserCheckInAsync(Guid userId, Guid eventCheckInId);
        public Task<int> GetTotalJoinerCountByClubIdAsync(Guid clubId);

        //  Etkinliğe katılan kullanıcıları çeker 
        Task<List<Guid>> GetUsersJoinedToEventAsync(Guid eventId);
    }
}
