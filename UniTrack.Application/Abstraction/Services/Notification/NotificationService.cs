using Microsoft.AspNetCore.SignalR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Notification;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using UniTrack.Persistence.Repositories;

public class NotificationService : INotificationService
{

    private readonly IHubContext<NotificationHub> _hubServices;
    private readonly IEventUserRepository eventUserRepository;
    private readonly IUserClubRepository userClubRepository;
    private readonly ILocalizationService localizationService;
    private readonly INotificationRepository notificationRepository;
    private readonly IUserNotificationRepository userNotificationRepository;
    private readonly IClubNotificationRepository clubNotificationRepository;
    private readonly IClubRepository clubRepository;
    private readonly ITargetNotificationRepository targetNotificationRepository;
    private readonly ITargetNotificationClubRepository targetNotificationClubRepository;


    public NotificationService(
        IHubContext<NotificationHub> hubServices,
        IEventUserRepository eventUserRepository,
        ILocalizationService localizationService,
        IUserClubRepository userClubRepository,
        INotificationRepository notificationRepository,
        IUserNotificationRepository userNotificationRepository,
        IClubNotificationRepository clubNotificationRepository,
        IClubRepository clubRepository,
        ITargetNotificationRepository targetNotificationRepository,
        ITargetNotificationClubRepository targetNotificationClubRepository)
    {
        _hubServices = hubServices;
        this.eventUserRepository = eventUserRepository;
        this.localizationService = localizationService;
        this.userClubRepository = userClubRepository;
        this.notificationRepository = notificationRepository;
        this.userNotificationRepository = userNotificationRepository;
        this.clubNotificationRepository = clubNotificationRepository;
        this.clubRepository = clubRepository;
        this.targetNotificationRepository = targetNotificationRepository;
        this.targetNotificationClubRepository = targetNotificationClubRepository;
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
        var users = await userClubRepository.GetUsersWithNotificationOpenForClubAsync(clubId);

        if (!users.Any())
            return;

        await CreateAndDispatchNotificationAsync(users,message,NotificationType.EventCreated,clubId);
    }

    public async Task ClubIsUpdateEventAsync(Guid clubId, Guid eventId, string message)
    {
        var users = await userClubRepository.GetUsersWithNotificationOpenForClubAsync(clubId);

        var joiners = await eventUserRepository.GetUsersJoinedToEventAsync(eventId);

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
        var users = await userClubRepository.GetUsersWithNotificationOpenForClubAsync(clubId);

        var joiners = await eventUserRepository.GetUsersJoinedToEventAsync(eventId);

        var targetUsers = users
            .Union(joiners)
            .Distinct()
            .ToList();

        if (!targetUsers.Any())
            return;

        await CreateAndDispatchNotificationAsync(targetUsers,message,NotificationType.EventDeleted,eventId);
    }

    // Grup bildiirmi 
    public async Task CreateAndDispatchNotificationAsync(List<Guid> userIds,string message,NotificationType type,Guid relatedEntityId)
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

    // Kulübe özel kaydedilen bilrim
    public async Task SendNotificationToClubAsync(Guid clubId,string message,NotificationType type,Guid? relatedEntityId = null)
    {
        // 1️⃣ Notification (1 kere)
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Message = message,
            Type = type,
            RelatedEntityId = relatedEntityId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await notificationRepository.AddAsync(notification);

        // 2️⃣ ClubNotification (1 kere)
        var clubNotification = new ClubNotification
        {
            Id = Guid.NewGuid(),
            ClubId = clubId,
            NotificationId = notification.Id,
            IsRead = false
        };

        await clubNotificationRepository.AddAsync(clubNotification);

        // 3️⃣ Real-time (Kulüp Paneli)
        await _hubServices.Clients
            .Group($"club-{clubId}")
            .SendAsync("ReceiveClubNotification", new
            {
                NotificationId = notification.Id,
                RelatedEntityId = relatedEntityId,
                Message = message,
                Type = type.ToString(),
                CreatedAt = notification.CreatedAt
            });
    }

    // Tüm kulüplere özel kaydedilen bildirim.
    public async Task CreateClubTargetNotificationAsync(List<Guid> clubIds,string title,string message,NotificationType type,Guid? relatedEntityId = null)
    {
        // 1️⃣ Notification (1 tane)
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Title = title,
            Message = message,
            Type = type,
            RelatedEntityId = relatedEntityId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await notificationRepository.AddAsync(notification);

        // 2️⃣ TargetNotification
        var target = new TargetNotification
        {
            Id = Guid.NewGuid(),
            NotificationId = notification.Id
        };

        await targetNotificationRepository.AddAsync(target);

        // 3️⃣ Target → Clubs
        var targetClubs = clubIds.Select(clubId => new TargetNotificationClub
        {
            Id = Guid.NewGuid(),
            TargetNotificationId = target.Id,
            ClubId = clubId
        }).ToList();

        await targetNotificationClubRepository.AddRangeAsync(targetClubs);
    }
}