using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Ban;
using UniTrack.Application.DTOs.Comment;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Application.Feature.Ban.Command;
using UniTrack.Application.Feature.Ban.Query;
using UniTrack.Application.Feature.Comment.Command;
using UniTrack.Application.Feature.Comment.Query;
using UniTrack.Application.Feature.Profile.Query;

namespace UniTrack.WebAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [ApiExplorerSettings(GroupName = "Admin")]
    public class ClubController : ControllerBase
    {
        private readonly IMediator mediator;
        public ClubController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("GetAllUser")]
        public async Task<ServiceResponse<List<GetAllUserQueryResponseDTO>>> GetAllUser([FromQuery] GetAllUserQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetUserAllComment")]
        public async Task<ServiceResponse<List<GetAllCommentByUserQueryResponseDTO>>> GetUserAllComment([FromQuery]GetAllCommentByUserQuery query)
        {
            return await mediator.Send(query); 
        }

        [HttpDelete("DeleteComment")]
        public async Task<ServiceResponse<string>> DeleteComment([FromBody] DeleteCommentCommand command)
        {
            return await mediator.Send(command);
        }

    }
}
