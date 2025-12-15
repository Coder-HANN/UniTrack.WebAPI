using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Comment;

namespace UniTrack.Application.Feature.Comment.Query
{
    public class GetAllCommentForEventQuery : IRequest<ServiceResponse<List<GetAllCommentForEventQueryResponseDTO>>>
    {
        public GetAllCommentForEventQuery() { }

        public Guid EventId { get; set; }
        public GetAllCommentForEventQuery(Guid eventId)
        {
            EventId = eventId;
        }

    }
}
