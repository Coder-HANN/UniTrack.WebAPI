using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.Feature.Contect.Command;

namespace UniTrack.WebAPI.Controllers.Public
{
    [ApiController]
    [Route("api/public/[controller]")]
    [ApiExplorerSettings(GroupName = "Public")]
    public class ContectController : ControllerBase
    {
        private readonly IMediator mediator;
        public ContectController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("ContectWithAdmin")]
        public async Task<ServiceResponse<string>> ContectWithAdmin([FromBody] SendContactMessageCommand command)
        {
            return await mediator.Send(command);
        }
    }
}
