using Microsoft.AspNetCore.SignalR;

namespace UniTrack.Application.Abstraction.Services.Notification
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(Guid userId, string message)
        {
            await Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
        }

        public async Task SendToAllNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }

        public async Task JoinClubGroup(Guid clubId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"club-{clubId}");
        }
    }
}