using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Comment.Command
{
    public class DeleteCommentForEventCommandHandler : IRequestHandler<DeleteCommentForEventCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly ICommentRepository commentRepository;
        public DeleteCommentForEventCommandHandler(
            ICurrentUserServices currentUserServices,
            ICommentRepository commentRepository)
        {
            this.currentUserServices = currentUserServices;
            this.commentRepository = commentRepository;
        }
        public async Task<ServiceResponse<string>> Handle(DeleteCommentForEventCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "User not authenticated"
                };
            }

            var role = currentUserServices.Role();
            if (role == null || role == Role.Club)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Yetkisiz kullanıcı"
                };
            }

            var comment = await commentRepository.GetCommentByEventAndUserIdAsync(request.CommentId, userId.Value);
            if (comment == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "Comment not found"
                };
            }
            await commentRepository.DeleteAsync(comment);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = "Comment deleted successfully"
            };
        }
    }
}
