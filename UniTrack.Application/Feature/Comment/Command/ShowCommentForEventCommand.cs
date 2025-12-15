using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Comment;

namespace UniTrack.Application.Feature.Comment.Command
{
    public class ShowCommentForEventCommand : IRequest<ServiceResponse<ShowCommentForEventResponseDTO>>
    {
        public Guid EventId { get; set; }
    }
}
