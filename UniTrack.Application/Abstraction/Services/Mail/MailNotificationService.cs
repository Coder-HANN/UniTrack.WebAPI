using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Mail;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

public class MailNotificationService : IMailNotificationService
{
    private readonly IUserDetailRepository userDetailRepository;
    private readonly IBackgroundMailQueue mailQueue;

    public MailNotificationService(
        IUserDetailRepository userDetailRepository,
        IBackgroundMailQueue mailQueue)
    {
        this.userDetailRepository = userDetailRepository;
        this.mailQueue = mailQueue;
    }

    public async Task SendAsync(Notification notification,List<int>? cityIds,List<Guid>? universityIds,List<int>? departmentIds,List<Guid>? clubIds)
    {
        if (!notification.Channels.Any(c => c.Channel == NotificationChannel.Email))
            return;

        var users = await userDetailRepository.GetUsersByTargetAsync(cityIds,universityIds,departmentIds,clubIds);

        foreach (var user in users)
        {
            if (string.IsNullOrEmpty(user.User.Email))
                continue;

            mailQueue.Enqueue(new MailQueueItem
            {
                To = user.User.Email,
                Subject = notification.Title,
                Body = notification.Message
            });
        }
    }

}
