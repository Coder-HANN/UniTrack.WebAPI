using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Notification;
using UniTrack.Application.Feature.Notification.Command;
using UniTrack.Application.Feature.Notification.Query;

namespace UniTrack.WebAPI.Controllers.Club
{
    [ApiController]
    [Route("api/club/[controller]")]
    [ApiExplorerSettings(GroupName = "Club")]
    public class NotificationController : Controller
    {
        private readonly IMediator mediator;
        public NotificationController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("GetClubAllNotification")]
        public async Task<ServiceResponse<List<NotificationListResponse>>> GetClubAllNotification([FromQuery] GetClubAllNotificationQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpPost("ClubNotificationRead")]
        public Task<ServiceResponse<string>> ClubNotificationRead([FromBody] MarkNotificationAsReadCommand command)
        {
            return mediator.Send(command);
        }

        [HttpPost("ClubReadAllNotification")]
        public Task<ServiceResponse<string>> ClubReadAllNotification([FromBody] MarkAllNotificationsAsReadCommand command)
        {
            return mediator.Send(command);
        }
    }
}