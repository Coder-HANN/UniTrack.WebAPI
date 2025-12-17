using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Comment.Command
{
    public class DeleteCommentForEventCommandHandler : IRequestHandler<DeleteCommentForEventCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly ICommentRepository commentRepository;
        private readonly ILocalizationService localizationService;
        public DeleteCommentForEventCommandHandler(
            ICurrentUserServices currentUserServices,
            ICommentRepository commentRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.commentRepository = commentRepository;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<string>> Handle(DeleteCommentForEventCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized),
                };
            }

            var role = currentUserServices.Role();
            if (role == null || role == Role.Club)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            var comment = await commentRepository.GetCommentByEventAndUserIdAsync(request.CommentId, userId.Value);
            if (comment == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.CommentNotFound),
                };
            }
            await commentRepository.DeleteAsync(comment);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = await localizationService.Get(ValidationKeys.CommentDeleted),
            };
        }
    }
}
