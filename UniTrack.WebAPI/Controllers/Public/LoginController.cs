using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Auth;
using UniTrack.Application.Feature.ActiveDetail.Query;
using UniTrack.Application.Feature.Auth.Command;

namespace UniTrack.WebAPI.Controllers.Public
{
    [ApiController]
    [Route("api/public/[controller]")]
    [ApiExplorerSettings(GroupName = "Public")]
    public class LoginController : ControllerBase
    {
        private readonly IMediator mediator;
        public LoginController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("Login")]
        public async Task<ServiceResponse<LoginResponseDTO>> Login([FromBody] LoginCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpGet("total-user-count")]
        public async Task<long> TotalUserCount([FromQuery] GetUserCountQuery query)
        {
            return await mediator.Send(query);
        }
    }
}
