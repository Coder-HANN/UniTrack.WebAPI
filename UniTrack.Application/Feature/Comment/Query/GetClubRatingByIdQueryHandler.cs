// UniTrack.Application/Feature/Comment/Query/GetClubRatingByIdQueryHandler.cs
using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Comment;

namespace UniTrack.Application.Feature.Comment.Query
{
    public class GetClubRatingByIdQueryHandler : IRequestHandler<GetClubRatingByIdQuery, ServiceResponse<ShowCommentForClubResponseDTO>>
    {
        private readonly ICommentRepository commentRepository;

        public GetClubRatingByIdQueryHandler(ICommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
        }

        public async Task<ServiceResponse<ShowCommentForClubResponseDTO>> Handle(GetClubRatingByIdQuery request, CancellationToken cancellationToken)
        {
            var (average, count) = await commentRepository.GetClubAverageRatingAsync(request.ClubId);

            return new ServiceResponse<ShowCommentForClubResponseDTO>
            {
                Data = new ShowCommentForClubResponseDTO
                {
                    Point = average,
                    CommentCount = count
                },
                IsSuccess = true,
                Message = null
            };
        }
    }
}