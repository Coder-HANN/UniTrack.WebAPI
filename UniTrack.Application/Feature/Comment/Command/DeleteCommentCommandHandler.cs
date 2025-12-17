using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Comment.Command
{
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly ICommentRepository commentRepository;
        private readonly ILocalizationService localizationService;
        public DeleteCommentCommandHandler(ICurrentUserServices currentUserServices,
            ICommentRepository commentRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.commentRepository = commentRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            
            var role = currentUserServices.Role();
            if (role != Role.Admin)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            var comment = await commentRepository.GetCommentIdAsync(request.CommentId);

            await commentRepository.DeleteAsync(comment);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = await localizationService.Get(ValidationKeys.CommentDeleted),
                Data = null
            };
        }
    }
}
