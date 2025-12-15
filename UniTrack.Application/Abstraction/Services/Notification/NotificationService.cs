using Microsoft.AspNetCore.SignalR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Notification;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

public class NotificationService : INotificationService
{

    private readonly IHubContext<NotificationHub> _hubServices;
    private readonly INotificationRepository notificationRepository;

    public NotificationService(IHubContext<NotificationHub> hubServices,INotificationRepository notificationRepository)
    {
        _hubServices = hubServices;
        this.notificationRepository = notificationRepository;
    }

    public async Task SendToUserAsync(Guid userId, string message)
    {
       await _hubServices.Clients.User(userId.ToString()).SendAsync("ReceiveNotification",message);  
    }

    public async Task SendToAllAsync(string message)
    {
        await _hubServices.Clients.All.SendAsync("", message);
    }

    public async Task ClubIsCreateEventAsync(Guid clubId, string message)
    {
        var users = await notificationRepository.GetUsersWithNotificationOpenForClubAsync(clubId);

        foreach (var userId in users)
        {
            await PersistAndSendRealTimeNotificationAsync(userId,"Yeni Etkinlik Oluşturuldu",message,NotificationType.EventCreated,relatedEntityId: clubId
            );
        }
    }
    public async Task ClubIsUpdateEventAsync(Guid clubId, string message)
    {
        var users = await notificationRepository.GetUsersWithNotificationOpenForClubAsync(clubId);
        foreach (var userId in users)
        {
            await PersistAndSendRealTimeNotificationAsync(
                userId,
                "Etkinlik Güncellendi",
                message,
                NotificationType.EventUpdated,
                relatedEntityId: clubId
            );
        }
    }

    public async Task ClubIsDeleteEventAsync(Guid clubId, string message)
    {
        var users = await notificationRepository.GetUsersWithNotificationOpenForClubAsync(clubId);

        foreach (var userId in users)
        {
            await PersistAndSendRealTimeNotificationAsync(
                userId,
                "Etkinlik Silindi",
                message,
                NotificationType.EventDeleted,
                relatedEntityId: clubId
            );
        }
    }


    // --------------------------------------------------------------------
    // KALICI OLARAK KAYDET + REAL-TIME GÖNDER
    // --------------------------------------------------------------------
    public async Task PersistAndSendRealTimeNotificationAsync(Guid userId,string title,string message,NotificationType notificationType,Guid relatedEntityId)
    {
        // Veritabanına kaydet
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = notificationType,
            RelatedEntityId = relatedEntityId,
            CreatedAt = DateTimeOffset.UtcNow,
            IsRead = false
        };

        await notificationRepository.AddAsync(notification);

        // Real-time bildirimi gönder
        await _hubServices.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", new
            {
                NotificationId = notification.Id,
                Title = title,
                Message = message,
                Type = notificationType.ToString(),
                RelatedEntityId = relatedEntityId,
                CreatedAt = notification.CreatedAt
            });
    }
}