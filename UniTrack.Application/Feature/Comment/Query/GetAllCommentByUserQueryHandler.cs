using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Comment;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Comment.Query
{
    public class GetAllCommentByUserQueryHandler : IRequestHandler<GetAllCommentByUserQuery, ServiceResponse<List<GetAllCommentByUserQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly ICommentRepository commentRepository;

        public GetAllCommentByUserQueryHandler(
            ICurrentUserServices currentUserServices,
            ICommentRepository commentRepository)
        {
            this.currentUserServices = currentUserServices;
            this.commentRepository = commentRepository;
        }

        public async Task<ServiceResponse<List<GetAllCommentByUserQueryResponseDTO>>> Handle(GetAllCommentByUserQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return new ServiceResponse<List<GetAllCommentByUserQueryResponseDTO>>
                {

                    IsSuccess = false,
                    Data = null,
                    Message = "Geçersiz kulllanıcı"
                };
            }

            var role = currentUserServices.Role();

            if (role == null || role == Role.Club)
            {
                return new ServiceResponse<List<GetAllCommentByUserQueryResponseDTO>>
                { 
                     IsSuccess = false,
                     Data = null,
                     Message = "Yetkisiz kullanıcı"
                };
            }

            var commants = await commentRepository.GetAllCommentsByUserIdAsync(userId.Value);

            if (commants == null)
            {

                return new ServiceResponse<List<GetAllCommentByUserQueryResponseDTO>>
                { 

                    IsSuccess = false,
                    Data = null,
                    Message = "Yorum yok"
                
                };
            }

            var responses = commants.Select(c => new GetAllCommentByUserQueryResponseDTO
            {
                
                    EventId = c.EventId,
                    ClubId = c.ClubId,
                    CommentText = c.Description,
                    Point = c.Point,
                    CommentId = c.Id
                
            }).ToList();

            return new ServiceResponse<List<GetAllCommentByUserQueryResponseDTO>>
            { 
                IsSuccess = true,
                Data = responses,
                Message = "Yorumlar getirildi"
            
            };
        }
    }
}
