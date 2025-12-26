using Microsoft.AspNetCore.SignalR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Notification;
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
    // bireysel geçici bildirim
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
        var users = await userClubRepository
            .GetUsersWithNotificationOpenForClubAsync(clubId);

        if (!users.Any())
            return;

        await CreateAndDispatchNotificationAsync(
            users,
            message,
            NotificationType.EventCreated,
            clubId
        );
    }

    public async Task ClubIsUpdateEventAsync(Guid clubId, Guid eventId, string message)
    {
        var users = await userClubRepository
            .GetUsersWithNotificationOpenForClubAsync(clubId);

        var joiners = await eventUserRepository
            .GetUsersJoinedToEventAsync(eventId);

        var targetUsers = users
            .Union(joiners)
            .Distinct()
            .ToList();

        if (!targetUsers.Any())
            return;

        await CreateAndDispatchNotificationAsync(
            targetUsers,
            message,
            NotificationType.EventUpdated,
            eventId
        );
    }

    public async Task ClubIsDeleteEventAsync(Guid clubId, Guid eventId, string message)
    {
        var users = await userClubRepository
            .GetUsersWithNotificationOpenForClubAsync(clubId);

        var joiners = await eventUserRepository
            .GetUsersJoinedToEventAsync(eventId);

        var targetUsers = users
            .Union(joiners)
            .Distinct()
            .ToList();

        if (!targetUsers.Any())
            return;

        await CreateAndDispatchNotificationAsync(
            targetUsers,
            message,
            NotificationType.EventDeleted,
            eventId
        );
    }

    private async Task CreateAndDispatchNotificationAsync(List<Guid> userIds,string message,NotificationType type,Guid relatedEntityId)
    {
        // Notification (1 KERE)
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Message = message,
            Type = type,
            RelatedEntityId = relatedEntityId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await notificationRepository.AddAsync(notification);

        //  UserNotification (N KERE)
        var userNotifications = userIds.Select(userId =>
            new UserNotification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                NotificationId = notification.Id,
                IsRead = false
            }).ToList();

        await userNotificationRepository.AddRangeAsync(userNotifications);

        // Real-time gönder
        foreach (var userId in userIds)
        {
            await _hubServices.Clients
                .User(userId.ToString())
                .SendAsync("ReceiveNotification", new
                {
                    NotificationId = notification.Id,
                    RelatedEntityId = relatedEntityId,
                    Message = message,
                    Type = type.ToString(),
                    CreatedAt = notification.CreatedAt
                });
        }
    }

    // Kişiye özel bildirim gönderir ve kalıcı bildirim içerir.
    public async Task SendDirectNotificationAsync(Guid userId,string message,NotificationType type,Guid? relatedEntityId = null)
    {
        // 1️⃣ Notification (1 tane – bireysel)
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Message = message,
            Type = type,
            RelatedEntityId = relatedEntityId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await notificationRepository.AddAsync(notification);

        // 2️⃣ UserNotification (1 tane)
        var userNotification = new UserNotification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            NotificationId = notification.Id,
            IsRead = false
        };

        await userNotificationRepository.AddAsync(userNotification);

        // 3️⃣ Real-time gönder
        await _hubServices.Clients
            .User(userId.ToString())
            .SendAsync("ReceiveNotification", new
            {
                NotificationId = notification.Id,
                RelatedEntityId = notification.RelatedEntityId,
                Message = message,
                Type = type.ToString(),
                CreatedAt = notification.CreatedAt
            });
    }
}