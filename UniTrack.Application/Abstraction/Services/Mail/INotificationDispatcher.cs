namespace UniTrack.Application.Abstraction.Services.Mail
{
    public interface INotificationDispatcher
    {
        Task DispatchAsync(Domain.Entities.Notification notification,List<int>? cityIds,List<Guid>? universityIds,List<int>? departmentIds,List<Guid>? clubIds);

    }

}
