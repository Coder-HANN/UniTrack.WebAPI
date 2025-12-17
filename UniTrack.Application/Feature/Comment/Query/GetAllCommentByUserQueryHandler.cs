using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Comment;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Comment.Query
{
    public class GetAllCommentByUserQueryHandler : IRequestHandler<GetAllCommentByUserQuery, ServiceResponse<List<GetAllCommentByUserQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly ICommentRepository commentRepository;
        private readonly ILocalizationService localizationService;

        public GetAllCommentByUserQueryHandler(
            ICurrentUserServices currentUserServices,
            ICommentRepository commentRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.commentRepository = commentRepository;
            this.localizationService = localizationService;
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
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            var role = currentUserServices.Role();

            if (role == null || role == Role.Club)
            {
                return new ServiceResponse<List<GetAllCommentByUserQueryResponseDTO>>
                { 
                     IsSuccess = false,
                     Data = null,
                     Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            var commants = await commentRepository.GetAllCommentsByUserIdAsync(userId.Value);

            if (commants == null)
            {

                return new ServiceResponse<List<GetAllCommentByUserQueryResponseDTO>>
                { 

                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.CommentNotFound)
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
                Message = null
            };
        }
    }
}
