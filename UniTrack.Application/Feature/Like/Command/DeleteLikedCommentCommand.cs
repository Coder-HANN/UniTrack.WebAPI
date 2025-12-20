using MediatR;

namespace UniTrack.Application.Feature.Like.Command
{
    public class DeleteLikedCommentCommand : IRequest<string>
    {
        public Guid CommentId { get; set; }
    }
}
