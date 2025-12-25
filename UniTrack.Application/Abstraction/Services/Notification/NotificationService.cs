using Microsoft.AspNetCore.SignalR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Notification;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

public class NotificationService : INotificationService
{

    private readonly IHubContext<NotificationHub> _hubServices;
    private readonly IEventUserRepository eventUserRepository;
    private readonly IUserClubRepository userClubRepository;
    private readonly ILocalizationService localizationService;
    private readonly INotificationRepository notificationRepository;
    private readonly IUserNotificationRepository userNotificationRepository;

    public NotificationService(
        IHubContext<NotificationHub> hubServices,
        IEventUserRepository eventUserRepository,
        ILocalizationService localizationService,
        IUserClubRepository userClubRepository,
        INotificationRepository notificationRepository,
        IUserNotificationRepository userNotificationRepository)
    {
        _hubServices = hubServices;
        this.eventUserRepository = eventUserRepository;
        this.localizationService = localizationService;
        this.userClubRepository = userClubRepository;
        this.notificationRepository = notificationRepository;
        this.userNotificationRepository = userNotificationRepository;
    }

    public async Task SendToUserAsync(Guid userId, string message)
    {
        await _hubServices.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
    }

    public async Task SendToAllAsync(string message)
    {
        await _hubServices.Clients.All.SendAsync("", message);
    }

    public async Task ClubIsCreateEventAsync(Guid clubId, string message)
    {
        var users = await userClubRepository.GetUsersWithNotificationOpenForClubAsync(clubId);

        foreach (var userId in users)
        {
            await PersistAndSendRealTimeNotificationAsync(userId, message, NotificationType.EventCreated, clubId);
        }
    }
    public async Task ClubIsUpdateEventAsync(Guid clubId,Guid eventId, string message)
    {
        var users = await userClubRepository.GetUsersWithNotificationOpenForClubAsync(clubId);

        var joiner = await eventUserRepository.GetUsersJoinedToEventAsync(eventId);

        var targetUsers = users // Etkinliğe katıldıysa veya bildirimleri açıksa
        .Union(joiner)
        .Distinct();

        foreach (var userId in targetUsers)
        {
            await PersistAndSendRealTimeNotificationAsync(userId,message,NotificationType.EventUpdated,clubId);
        }
    }

    public async Task ClubIsDeleteEventAsync(Guid clubId, Guid eventId, string message)
    {
        var users = await userClubRepository.GetUsersWithNotificationOpenForClubAsync(clubId);
        var joiner = await eventUserRepository.GetUsersJoinedToEventAsync(eventId);

        var targetUsers = users // Etkinliğe katıldıysa veya bildirimleri açıksa
        .Union(joiner)
        .Distinct();

        foreach (var userId in targetUsers)
        {
            await PersistAndSendRealTimeNotificationAsync(userId,message,NotificationType.EventDeleted, clubId);
        }
    }


    // --------------------------------------------------------------------
    // KALICI OLARAK KAYDET + REAL-TIME GÖNDER
    // --------------------------------------------------------------------
    public async Task PersistAndSendRealTimeNotificationAsync(Guid userId,string message,NotificationType type,Guid relatedEntityId)
    {
        // Veritabanına kaydet
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Message = message,
            Type = type,
            RelatedEntityId = relatedEntityId,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        await notificationRepository.AddAsync(notification);

        var userNotification = new UserNotification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            NotificationId = notification.Id,
            IsRead = false,
        };

        await userNotificationRepository.AddAsync(userNotification);

        // Real-time bildirimi gönder
        await _hubServices.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", new
        {
            NotificationId = notification.Id,
            RelatedEntityId = notification.RelatedEntityId,
            UserId = userId,
            Message = message,
            Type = type.ToString(),
            CreatedAt = notification.CreatedAt
        });
    }
}