using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface INotificationRepository : BaseEntityRepository<Notification>
    {
        // 1. Kulübü takip eden ve bildirimi açık olan kullanıcıları çeker (UserClub kullanır)
        Task<List<Guid>> GetUsersWithNotificationOpenForClubAsync(Guid clubId);

        // 2. Etkinliğe katılan kullanıcıları çeker (EventUser kullanır)
        Task<List<Guid>> GetUsersJoinedToEventAsync(Guid eventId);

        // 3. Kullanıcının okumadığı bildirimleri çeker
        Task<List<Notification>> GetUserNotificationsAsync(Guid userId, int take = 50);


        // Tüm bildirimleri çeker
        Task<List<Notification>> GetUserAllNotification(Guid userId);
    }                                                                                               
}