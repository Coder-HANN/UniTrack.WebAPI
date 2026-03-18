using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.Feature.Like.Command;

namespace UniTrack.WebAPI.Controllers.Public
{
    [ApiController]
    [Route("api/public/Like")]
    public class LikeController : Controller
    {
        private readonly IMediator mediator;
        public LikeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("CreateLike")]
        public async Task<ServiceResponse<string>> CreateLike([FromBody] CreateLikedCommentCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost("RemoveLike")]
        public async Task<ServiceResponse<string>> RemoveLike([FromBody] DeleteLikedCommentCommand command)
        {
            return await mediator.Send(command);

        }
    }
}
