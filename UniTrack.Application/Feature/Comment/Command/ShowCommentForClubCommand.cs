using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Comment;

namespace UniTrack.Application.Feature.Comment.Command
{
    public class ShowCommentForClubCommand : IRequest<ServiceResponse<ShowCommentForClubResponseDTO>>
    {
        public Guid ClubId { get; set; }
    }
}
