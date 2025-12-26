using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Abstraction.Services.Mail;
using UniTrack.Application.Common;
using UniTrack.Application.Feature.Notification.Command;

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

    }
}