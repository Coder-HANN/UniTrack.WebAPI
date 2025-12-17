using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Comment;

namespace UniTrack.Application.Feature.Comment.Command
{
    public class ShowCommentForClubCommandHandler : IRequestHandler<ShowCommentForClubCommand, ServiceResponse<ShowCommentForClubResponseDTO>>
    {
        private readonly ICommentRepository commentRepository;
        

        public ShowCommentForClubCommandHandler(ICommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
        }

        public async Task<ServiceResponse<ShowCommentForClubResponseDTO>> Handle(ShowCommentForClubCommand request, CancellationToken cancellationToken)
        {
            var calculatedAverage = await commentRepository.GetClubAverageRatingAsync(request.ClubId);

            if(calculatedAverage == null)
            {
                calculatedAverage = 0;
            }

            var responseDto = new ShowCommentForClubResponseDTO
            {
                
                Point = calculatedAverage
            };

            return new ServiceResponse<ShowCommentForClubResponseDTO>
            {
                Data = responseDto,
                IsSuccess = true,
                Message = null
            };
        }
    }
}