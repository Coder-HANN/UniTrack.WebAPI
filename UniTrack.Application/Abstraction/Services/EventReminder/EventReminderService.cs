using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Notification;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Abstraction.Services.EventReminder
{
    public class EventReminderService : IEventReminderService
    {
        private readonly IEventUserRepository eventUserRepository;
        private readonly INotificationService notificationService;

        public EventReminderService(
            IEventUserRepository eventUserRepository,
            INotificationService notificationService)
        {
            this.eventUserRepository = eventUserRepository;
            this.notificationService = notificationService;
        }

        public async Task SendEventReminder(Guid eventId, string title, string message)
        {
            // 1️⃣ Etkinliğe katılan kullanıcıları al
            var targetUserIds = await eventUserRepository
                .GetUsersJoinedToEventAsync(eventId);

            // 2️⃣ Her kullanıcıya bireysel ve kalıcı reminder bildirimi gönder
            foreach (var userId in targetUserIds)
            {
                await notificationService.SendDirectNotificationAsync(
                    userId: userId,
                    message: message,
                    type: NotificationType.EventReminder,
                    relatedEntityId: eventId // event sayfasına yönlendirme için
                );
            }
        }
    }
}
