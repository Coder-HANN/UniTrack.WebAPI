using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Like.Command
{
    public class DeleteLikedCommentCommandHandler : IRequestHandler<DeleteLikedCommentCommand, string>
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

        public async Task<string> Handle(DeleteLikedCommentCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            var clubId = currentUserServices.CurrentClub();

            if (userId == null || clubId == null)
            {
                return await localizationService.Get(ValidationKeys.NotAuthorized);
            }

            var commentLike = await likeRepository.GetAsync(c => c.Id == request.CommentId && c.UserId == userId);

            if (commentLike == null)
            {
                return await localizationService.Get(ValidationKeys.CommentNotFound);
            }
            await likeRepository.DeleteAsync(commentLike);

            return await localizationService.Get("Beğeni silindi.");

        }
    }
}
