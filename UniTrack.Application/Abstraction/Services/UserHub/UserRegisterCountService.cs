using Microsoft.AspNetCore.SignalR;
using UniTrack.Application.Abstraction.Repositories;


namespace UniTrack.Application.Abstraction.Services.UserHub;

public class RegistrationNotifierService : IUserRegisterCountService
{
    private readonly IHubContext<UserCountHub> _hubContext;
    private readonly IUserRepository _userRepository;

    public RegistrationNotifierService(
        IHubContext<UserCountHub> hubContext,
        IUserRepository userRepository)
    {
        _hubContext = hubContext;
        _userRepository = userRepository;
    }

    public async Task NotifyUserCountUpdatedAsync()
    {
        try
        {
            // 1. Kullanıcı sayısını servis içinde çek
            long totalUserCount = await _userRepository.GetUserCountAsync();

            // 2. Yayınlama işlemini servis içinde yap
            await _hubContext.Clients.All.SendAsync("UserCountUpdated", totalUserCount);
        }
        catch (Exception ex)
        {
            // Yayınlama hatalarını burada loglayabilirsiniz.
            // Örneğin ILogger kullanarak loglama yapın.
            Console.WriteLine($"SignalR yayını başarısız: {ex.Message}");
        }
    }
}