using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Like.Command
{
    public class CreateLikedCommentCommandHandler : IRequestHandler<CreateLikedCommentCommand, string>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;
        private readonly ICommentRepository commentRepository;
        private readonly ILikeRepository likeRepository;
        public CreateLikedCommentCommandHandler(
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService,
            ICommentRepository commentRepository,
            ILikeRepository likeRepository)
        {
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
            this.commentRepository = commentRepository;
            this.likeRepository = likeRepository;
        }

        public async Task<string> Handle(CreateLikedCommentCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            var clubId = currentUserServices.CurrentClub();

            if (userId == null && clubId == null)
            {
                return await localizationService.Get(ValidationKeys.NotAuthorized);
            }

            var comment = await commentRepository.GetAsync(c => c.Id == request.CommentId);

            if (comment == null)
            {
                return await localizationService.Get(ValidationKeys.CommentNotFound);
            }

            var isAgain = await likeRepository.GetAsync(l =>
                l.CommentId == request.CommentId &&
                (
                    (userId != null && l.UserId == userId) ||
                    (clubId != null && l.ClubId == clubId)
                )
            );

            if (isAgain != null)
            {
                return await localizationService.Get(ValidationKeys.AlreadyLiked);
            }

            await likeRepository.AddAsync(new Domain.Entities.Like
            {
                CommentId = request.CommentId,
                UserId = userId,
                ClubId = clubId
            });

            await commentRepository.IncrementLikeCountAsync(request.CommentId);

            return await localizationService.Get("İşlem başarılı") ;
        }
    }
}
