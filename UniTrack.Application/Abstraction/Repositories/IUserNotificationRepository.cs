using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IUserNotificationRepository : IBaseEntityRepository<UserNotification>
    {
        Task AddRangeAsync(List<UserNotification> userNotifications);
        public Task<List<UserNotification>> GetUserAllNotification(Guid userId);
        // Kullanıcının okumadığı bildirimleri çeker
        Task<List<UserNotification>> GetUserNotificationsAsync(Guid userId, int take = 50);

    }
}
