namespace UniTrack.Application.Abstraction.Services.UserHub
{
    public interface IUserRegisterCountService
    {
        Task NotifyUserCountUpdatedAsync();
    }
}
