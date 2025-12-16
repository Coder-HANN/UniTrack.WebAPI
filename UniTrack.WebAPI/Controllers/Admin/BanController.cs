using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Ban;
using UniTrack.Application.Feature.Ban.Command;
using UniTrack.Application.Feature.Ban.Query;

namespace UniTrack.WebAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [ApiExplorerSettings(GroupName = "Admin")]
    public class BanController : ControllerBase
    {
        private readonly IMediator mediator;
        public BanController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("UserBan")]
        public async Task<ServiceResponse<string>> UserBan([FromBody] BanForUserCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost("ClubBan")]
        public async Task<ServiceResponse<string>> ClubBan([FromBody] BanForClubCommand command)
        {
            return await mediator.Send(command);
        }
        [HttpGet("ClubBanList")]
        public async Task<ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>>> ClubBanList([FromQuery] GetBanedClubQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("UserBanList")]
        public async Task<ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>>> UserBanList([FromQuery] GetBanedUserQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpPost("RemoveBan")]
        public async Task<ServiceResponse<string>> RemoveBan([FromBody] BanDeleteCommand command)
        {
            return await mediator.Send(command);
        }
    }
}
