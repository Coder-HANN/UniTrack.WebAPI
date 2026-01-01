using UniTrack.Application.Abstraction.Services.Mail;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

public class NotificationDispatcher : INotificationDispatcher
{
    private readonly IMailNotificationService mailService;
    // ileride: IPushNotificationService, ISmsNotificationService

    public NotificationDispatcher(IMailNotificationService mailService)
    {
        this.mailService = mailService;
    }

    public async Task DispatchAsync(Notification notification,List<int>? cityIds,List<Guid>? universityIds,List<int>? departmentIds,List<Guid>? clubIds)
    {
        // Channel routing burada yapılır
        if (notification.Channels.Any(c => c.Channel == NotificationChannel.Email))
        {
            await mailService.SendAsync(
                notification,
                cityIds,
                universityIds,
                departmentIds,
                clubIds);
        }
    }
}