using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Application.Feature.Profile.Command;

namespace UniTrack.WebAPI.Controllers.Public
{
    [ApiController]
    [Route("api/public/[controller]")]
    [ApiExplorerSettings(GroupName = "Public")]
    public class UserController : ControllerBase
    {
        private readonly IMediator mediator;
        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPut("UserProfileUpdate")]

        public async Task<ServiceResponse<UserProfileUpdateResponseDTO>> UserProfileUpdate([FromBody] UserProfileUpdateCommand command) 
        {
            return await mediator.Send(command);
        }


    }
}
