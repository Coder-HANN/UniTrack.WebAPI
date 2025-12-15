using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Comment;

namespace UniTrack.Application.Feature.Comment.Query
{
    public class GetAllCommentForEventQueryHandler : IRequestHandler<GetAllCommentForEventQuery, ServiceResponse<List<GetAllCommentForEventQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly ICommentRepository commentRepository;
        public GetAllCommentForEventQueryHandler(
            ICurrentUserServices currentUserServices,
            ICommentRepository commentRepository)
        {
            this.currentUserServices = currentUserServices;
            this.commentRepository = commentRepository;
        }
        public async Task<ServiceResponse<List<GetAllCommentForEventQueryResponseDTO>>> Handle(GetAllCommentForEventQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return new ServiceResponse<List<GetAllCommentForEventQueryResponseDTO>> {
                    
                        IsSuccess = false,
                        Data = null,
                        Message = "Unauthorized"
                    
                };
            }
            var comments = await commentRepository.GetAllCommentByEventIdAsync(request.EventId);
            if (comments == null || comments.Count == 0)
            {
                return new ServiceResponse<List<GetAllCommentForEventQueryResponseDTO>> {
                    
                        IsSuccess = false,
                        Data = null,
                        Message = "No comments found"
                    
                };
            }

            var responses = comments.Select(c => new GetAllCommentForEventQueryResponseDTO
            {
                    Name = c.User.UserDetail.Name,
                    Surname = c.User.UserDetail.Surname,
                    CreatedDate = c.CreatedDate,
                    Point = c.Point,
                    Description = c.Description

            }).ToList();
            return new ServiceResponse<List<GetAllCommentForEventQueryResponseDTO>> {
                
                    IsSuccess = true,
                    Data = responses,
                    Message = "Comments retrieved successfully"
                
            };
        }
    }
}
