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

        public NotificationController(IMailService mailService)
        {
            mediator = mediator;
        }


        [HttpPost("NotificationRead")]
        public Task<ServiceResponse<string>> NotificationRead([FromBody] MarkNotificationAsReadCommand command)
        {
            return mediator.Send(command);
        }

    }
}