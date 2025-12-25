using UniTrack.Domain.Enums;

namespace UniTrack.Application.Abstraction.Services.Notification
{
    public interface INotificationService
    {
        Task ClubIsCreateEventAsync(Guid clubId, string message); // kulüp etkinlik oluşturursa bunu kontrol et
        Task ClubIsDeleteEventAsync(Guid clubId,Guid eventId, string message); // kulüp etkinlik silerse bunu kontrol et
        Task ClubIsUpdateEventAsync(Guid clubId,Guid eventId, string message); // kulüp etkinlik güncellerse bunu kontrol et
        Task PersistAndSendRealTimeNotificationAsync(Guid userId, string message, NotificationType notificationType, Guid relatedEntityId); // Bireysel bildim
        Task SendToUserAsync(Guid userId, string message);
        Task SendToAllAsync(string message);
    }
}