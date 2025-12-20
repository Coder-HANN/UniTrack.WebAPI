using MediatR;

namespace UniTrack.Application.Feature.Like.Command
{
    public class CreateLikedCommentCommand : IRequest<string>
    {
        public Guid CommentId { get; set; }
    }
}
