using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Auth;
using UniTrack.Application.Feature.Auth.Command;

namespace UniTrack.WebAPI.Controllers.Public
{
    [ApiController]
    [Route("api/public/[controller]")]
    [ApiExplorerSettings(GroupName = "Public")]
    public class RegisterController : ControllerBase
    {
        private readonly IMediator mediator;
        public RegisterController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("ClubRegister")]
        public async Task<ServiceResponse<ClubRegisterResponseDTO>> ClubRegister([FromBody] ClubRegisterCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost("UserRegister")]
        public async Task<ServiceResponse<UserRegisterResponseDTO>> UserRegister([FromBody] UserRegisterCommand command)
        {
            return await mediator.Send(command);
        }
    }
}
