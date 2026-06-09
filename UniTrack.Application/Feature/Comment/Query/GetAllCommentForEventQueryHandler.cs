using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Comment;

namespace UniTrack.Application.Feature.Comment.Query
{
    public class GetAllCommentForEventQueryHandler : IRequestHandler<GetAllCommentForEventQuery, ServiceResponse<List<GetAllCommentForEventQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly ICommentRepository commentRepository;
        private readonly ILocalizationService localizationService;
        public GetAllCommentForEventQueryHandler(
            ICurrentUserServices currentUserServices,
            ICommentRepository commentRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.commentRepository = commentRepository;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<List<GetAllCommentForEventQueryResponseDTO>>> Handle(GetAllCommentForEventQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            var clubId = currentUserServices.CurrentClub();

            bool authorized = false;

             if (userId != null)
            {
                authorized = true;
            }
            else if (clubId != null)
            {
                authorized = true;
            }else 
            {
                return new ServiceResponse<List<GetAllCommentForEventQueryResponseDTO>>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            var comments = await commentRepository.GetAllCommentByEventIdAsync(request.EventId);
            if (comments == null || comments.Count == 0)
            {
                return new ServiceResponse<List<GetAllCommentForEventQueryResponseDTO>> {
                    
                        IsSuccess = true,
                        Data = null,
                        Message = await localizationService.Get(ValidationKeys.CommentNotFound)

                };
            }

            var responses = comments.Select(c => new GetAllCommentForEventQueryResponseDTO
            {
                UserId = c.UserId,
                Name = c.User.UserDetail.Name,
                Surname = c.User.UserDetail.Surname,
                CreatedDate = c.CreatedDate,
                Point = c.Point,
                Description = c.Description,
                CommentId = c.Id,
                HelpfulCount = c.LikeCount,
                IsLiked = c.Likes != null && c.Likes.Any(l => l.UserId == userId),
                ProfileImageUrl = c.User.UserDetail.ProfileImageUrl,

            }).ToList();
            return new ServiceResponse<List<GetAllCommentForEventQueryResponseDTO>> {
                
                    IsSuccess = true,
                    Data = responses,
                    Message = null
            };
        }
    }
}
