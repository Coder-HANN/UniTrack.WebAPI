// UniTrack.Application/Feature/Comment/Query/GetClubRatingByIdQuery.cs
using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Comment;

namespace UniTrack.Application.Feature.Comment.Query
{
    public class GetClubRatingByIdQuery : IRequest<ServiceResponse<ShowCommentForClubResponseDTO>>
    {
        public Guid ClubId { get; set; }

        public GetClubRatingByIdQuery(Guid clubId)
        {
            ClubId = clubId;
        }
    }
}