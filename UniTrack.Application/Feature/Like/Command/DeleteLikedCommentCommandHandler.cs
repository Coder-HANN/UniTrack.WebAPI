using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Like.Command
{
    public class DeleteLikedCommentCommandHandler : IRequestHandler<DeleteLikedCommentCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILikeRepository likeRepository;
        private readonly ICommentRepository commentRepository;
        private readonly ILocalizationService localizationService;
        public DeleteLikedCommentCommandHandler(
            ICurrentUserServices currentUserServices,
            ILikeRepository likeRepository,
            ICommentRepository commentRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.likeRepository = likeRepository;
            this.commentRepository = commentRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(DeleteLikedCommentCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            var clubId = currentUserServices.CurrentClub();

            if (userId == null && clubId == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            var commentLike = await likeRepository.GetAsync(c =>
                c.CommentId == request.CommentId && // ✅ düzeltildi
                ((userId != null && c.UserId == userId) ||
                 (clubId != null && c.ClubId == clubId)));
            if (commentLike == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.CommentNotFound)
                };
            }
            await likeRepository.DeleteAsync(commentLike);
            await commentRepository.DecrementLikeCountAsync(request.CommentId);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Data = null,
                Message = await localizationService.Get("Beğeni silindi")
            };

        }
    }
}
