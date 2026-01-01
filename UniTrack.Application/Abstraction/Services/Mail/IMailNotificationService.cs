namespace UniTrack.Application.Abstraction.Services.Mail
{
    public interface IMailNotificationService
    {
        Task SendAsync(Domain.Entities.Notification notification, List<int>? cityIds, List<Guid>? universityIds, List<int>? departmentIds, List<Guid>? clubIds);

    }

}
