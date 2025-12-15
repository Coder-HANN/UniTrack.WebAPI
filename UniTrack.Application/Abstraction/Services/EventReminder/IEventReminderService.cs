namespace UniTrack.Application.Abstraction.Services.EventReminder
{
    public interface IEventReminderService
    {
        // Hangfire tarafından çağrılacak asıl hatırlatma gönderme metodu
        Task SendEventReminder(Guid eventId, string title, string message);
    }
}
