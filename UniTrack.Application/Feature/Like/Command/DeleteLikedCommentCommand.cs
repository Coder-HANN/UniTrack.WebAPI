using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Like.Command
{
    public class DeleteLikedCommentCommand : IRequest<ServiceResponse<string>>
    {
        public Guid CommentId { get; set; }
    }
}
