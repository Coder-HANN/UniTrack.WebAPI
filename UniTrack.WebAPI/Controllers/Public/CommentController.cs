using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Comment;
using UniTrack.Application.Feature.Comment.Command;
using UniTrack.Application.Feature.Comment.Query;

namespace UniTrack.WebAPI.Controllers.Public
{
    [ApiController]
    [Route("api/public/[controller]")]
    [ApiExplorerSettings(GroupName = "Public")]
    public class CommentController : ControllerBase
    {
        private readonly IMediator mediator;
        public CommentController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("CreateCommentForEvent")]
        public async Task<ServiceResponse<string>> CreateCommentForEvent([FromBody] CreateCommentForEventCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost("ShowCommentForClub")]
        public async Task<ServiceResponse<ShowCommentForClubResponseDTO>> ShowCommentForClub([FromBody] ShowCommentForClubCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost("ShowCommentForEvent")]
        public async Task<ServiceResponse<ShowCommentForEventResponseDTO>> ShowCommentForEvent([FromBody] ShowCommentForEventCommand command)
        {
            return await mediator.Send(command);
        }


        [HttpGet("GetAllEventComment")]
        public async Task<ServiceResponse<List<GetAllCommentForEventQueryResponseDTO>>> GetAllEventComment([FromQuery] GetAllCommentForEventQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetAllCommentbyUser")]

        public async Task<ServiceResponse<List<GetAllCommentByUserQueryResponseDTO>>> GetAllCommentbyUser([FromQuery] GetAllCommentByUserQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetClubRatingById")]
        public async Task<ServiceResponse<ShowCommentForClubResponseDTO>> GetClubRatingById([FromQuery] GetClubRatingByIdQuery query)
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
