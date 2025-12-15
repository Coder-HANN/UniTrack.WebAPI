using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Comment;

namespace UniTrack.Application.Feature.Comment.Query
{
    public class GetAllCommentByUserQuery : IRequest<ServiceResponse<List<GetAllCommentByUserQueryResponseDTO>>>
    {
        public GetAllCommentByUserQuery() {}
    }
}
