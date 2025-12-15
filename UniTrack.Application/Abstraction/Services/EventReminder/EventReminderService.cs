using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Notification;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Abstraction.Services.EventReminder
{
    public class EventReminderService : IEventReminderService
    {
        private readonly INotificationRepository notificationRepository;
        private readonly INotificationService notificationServices;
        public EventReminderService(
            INotificationRepository notificationRepository,
            INotificationService notificationServices)
        {
            this.notificationRepository = notificationRepository;
            this.notificationServices = notificationServices;
        }
        public async Task SendEventReminder(Guid eventId, string title, string message)
        {
            // 1. Etkinliğe katılan kullanıcıları çek (EventUser tablosu)
            var targetUserIds = await notificationRepository.GetUsersJoinedToEventAsync(eventId);

            // 2. Her kullanıcı için bireysel bildirim görevini tetikle
            foreach (var userId in targetUserIds)
            {
                // Burada tekrar kuyruğa atabiliriz veya direkt olarak kalıcılık metodunu çağırabiliriz (hata riski az olduğu için direkt çağıralım)
                await notificationServices.PersistAndSendRealTimeNotificationAsync(userId,title,message,NotificationType.EventReminder,eventId);
            }

        }
    }
}
