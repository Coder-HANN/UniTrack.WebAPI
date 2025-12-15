using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Comment;

namespace UniTrack.Application.Feature.Comment.Command
{
    public class ShowCommentForEventCommandHandler : IRequestHandler<ShowCommentForEventCommand, ServiceResponse<ShowCommentForEventResponseDTO>>
    {
        private readonly ICommentRepository commentRepository;

        public ShowCommentForEventCommandHandler(ICommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
        }

        public async Task<ServiceResponse<ShowCommentForEventResponseDTO>> Handle(ShowCommentForEventCommand request, CancellationToken cancellationToken)
        {
            var calculatedAverage = await commentRepository.GetEventAverageRatingAsync(request.EventId);

            if (calculatedAverage == null)
            {
                calculatedAverage = 0;
            }

            var responseDto = new ShowCommentForEventResponseDTO
            {

                Point = (float)Math.Round(calculatedAverage, 1)
            };

            return new ServiceResponse<ShowCommentForEventResponseDTO>
            {
                Data = responseDto,
                IsSuccess = true,
                Message = null
            };
        }
    }
}