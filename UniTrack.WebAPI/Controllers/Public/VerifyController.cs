using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.Feature.VerificationCode.Command;

namespace UniTrack.WebAPI.Controllers.Public
{
    [ApiController]
    [Route("api/public/[controller]")]
    [ApiExplorerSettings(GroupName = "Public")]
    public class VerifyController : ControllerBase
    {
        private readonly IMediator mediator;

        public VerifyController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("WriteCode")]

        public async Task<ServiceResponse<string>> WriteCode([FromBody] VerificationCommand command)
        {
            return await mediator.Send(command);
        }
    }
}
