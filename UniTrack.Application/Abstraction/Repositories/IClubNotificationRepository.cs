using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IClubNotificationRepository : IBaseEntityRepository<ClubNotification>
    {
        public Task<List<ClubNotification>> GetClubAllNotification(Guid clubId);

        // Kulüplerin okumadığı bildiirmleri döner
        public Task<List<ClubNotification>> GetClubNotificationsAsync(Guid clubId, int take = 50);
    }
}
