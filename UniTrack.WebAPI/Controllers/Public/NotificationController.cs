using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Abstraction.Services.Mail;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Notification;
using UniTrack.Application.Feature.Notification.Command;
using UniTrack.Application.Feature.Notification.Query;

namespace UniTrack.WebAPI.Controllers.Public
{
    [ApiController]
    [Route("api/public/[action]")]
    [ApiExplorerSettings(GroupName = "Public")]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator mediator;

        public NotificationController(IMediator mediator)
        {
            this.mediator = mediator;
        }


        [HttpPost("UserNotificationRead")]
        public async Task<ServiceResponse<string>> UserNotificationRead([FromBody] MarkNotificationAsReadCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost("UserReadAllNotification")]
        public async Task<ServiceResponse<string>> UserReadAllNotification([FromBody] MarkAllNotificationsAsReadCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpGet("GetUserNotifications")]
        public async Task<ServiceResponse<List<NotificationListResponse>>> GetUserNotifications([FromQuery] GetUserNotificationsQuery query)
        {
            return await mediator.Send(query);

        }
    }
}