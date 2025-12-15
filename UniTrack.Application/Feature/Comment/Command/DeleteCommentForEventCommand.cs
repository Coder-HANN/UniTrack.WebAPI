using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Comment.Command
{
    public class DeleteCommentForEventCommand : IRequest<ServiceResponse<string>>
    {
        public Guid CommentId { get; set; }
    }
}
