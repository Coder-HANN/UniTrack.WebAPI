using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Comment;

namespace UniTrack.Application.Feature.Comment.Command
{
    public class ShowCommentForClubCommandHandler : IRequestHandler<ShowCommentForClubCommand, ServiceResponse<ShowCommentForClubResponseDTO>>
    {
        private readonly ICommentRepository commentRepository;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;

        public ShowCommentForClubCommandHandler(
            ICommentRepository commentRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService)
        {
            this.commentRepository = commentRepository;
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<ShowCommentForClubResponseDTO>> Handle(ShowCommentForClubCommand request, CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();
            if (clubId == null)
            {
                return ServiceResponse<ShowCommentForClubResponseDTO>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));
            }
            var (average, count) = await commentRepository.GetClubAverageRatingAsync(clubId.Value);

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