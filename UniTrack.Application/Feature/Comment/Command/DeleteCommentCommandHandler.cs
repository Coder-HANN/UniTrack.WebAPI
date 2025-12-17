using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Comment.Command
{
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly ICommentRepository commentRepository;
        public DeleteCommentCommandHandler(ICurrentUserServices currentUserServices,
            ICommentRepository commentRepository)
        {
            this.currentUserServices = currentUserServices;
            this.commentRepository = commentRepository;
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
                    Message = "Yetkisiz kullanıcı"
                };
            }

            var comment = await commentRepository.GetCommentIdAsync(request.CommentId);

            await commentRepository.DeleteAsync(comment);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = "Comment deleted successfully",
                Data = null
            };
        }
    }
}
