using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Like.Command
{
    public class CreateLikedCommentCommand : IRequest<ServiceResponse<string>>
    {
        public Guid CommentId { get; set; }
    }
}
