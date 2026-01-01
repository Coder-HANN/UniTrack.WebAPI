using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Abstraction.Services.Mail;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Notification;
using UniTrack.Application.Feature.Notification.Command;
using UniTrack.Application.Feature.Notification.Query;

namespace UniTrack.WebAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "Admin")]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator mediator;

        public NotificationController(IMailService mailService)
        {
            mediator = mediator;
        }

        [HttpPost("PushNotification")]
        public Task<ServiceResponse<string>> PushNotification([FromBody] SendNotificationCommand command)
        { 
            return mediator.Send(command);
        }

        [HttpGet("SeeNotificationUserCount")]
        public Task<ServiceResponse<PreviewTargetNotificationResponseDTO>> SeeNotificationUserCount([FromQuery] PreviewTargetNotificationQuery query)
        {
            return mediator.Send(query);
        }

        [HttpPost("PushNotificationForUser")]
        public Task<ServiceResponse<bool>> PushNotificationForUser([FromBody] PushNotificationForUserCommand command)
        {
            return mediator.Send(command);
        }

        [HttpPost("push-UsersTarget")]
        public async Task<ServiceResponse<bool>> PushUsersTargetNotification(PushTargetNotificationCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost("push-target-club")]
        public async Task<ServiceResponse<bool>> PushTargetNotificationForClubs(PushTargetNotificationForClubsCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost("push-club")]
        public async Task<ServiceResponse<bool>> PushNotificationForClub(PushNotificationForClubCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost("PushAllUserSendNotificationsOrMail")]
        public async Task<ServiceResponse<string>> PushAllPeopleSendNotificationsOrMail([FromBody] SendNotificationCommand command )  // Burada mail atabilirsin veya sadece bildirim atabilirsin
        {
            return await mediator.Send(command);
        }
    }
}