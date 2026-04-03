using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Application.Feature.ActiveDetail.Query;
using UniTrack.Application.Feature.Profile.Command;
using UniTrack.Application.Feature.Profile.Query;

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

        [HttpGet("total-user-count")]
        public async Task<long> TotalUserCount([FromQuery] GetUserCountQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetUserProfileQuery")]
        public async Task<ServiceResponse<UserProfileUpdateResponseDTO>> GetUserProfile([FromQuery] GetUserProfileQuery query)
        {
            return await mediator.Send(query);

        }

        [HttpPost("Upload")]
        public async Task<ServiceResponse<UploadProfileImageResponseDTO>> Upload([FromForm] UploadProfileImageCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpDelete("Delete")]
        public async Task<ServiceResponse<string>> Delete([FromBody] DeleteProfileImageCommand command)
        {
            return await mediator.Send(command);
        }
    }
}
