using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.Feature.Event.Command;

namespace UniTrack.WebAPI.Controllers.Public
{
    [ApiController]
    [Route("api/public/[controller]")]
    [ApiExplorerSettings(GroupName = "Public")]
    public class EventLikeController : ControllerBase
    {
        private readonly IMediator mediator;

        public EventLikeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<ServiceResponse<string>> Like([FromBody] LikeEventCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpDelete]
        public async Task<ServiceResponse<string>> Unlike([FromBody] UnlikeEventCommand command)
        {
            return await mediator.Send(command);
        }
    }
}