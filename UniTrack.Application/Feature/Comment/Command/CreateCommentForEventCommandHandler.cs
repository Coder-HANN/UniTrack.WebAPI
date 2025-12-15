using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Comment.Command
{
    public class CreateCommentForEventCommandHandler : IRequestHandler<CreateCommentForEventCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventUserRepository eventUserRepository;
        private readonly ICommentRepository commentRepository;
        private readonly IEventRepository eventRepository;
        public CreateCommentForEventCommandHandler(
            ICurrentUserServices currentUserServices,
            IEventUserRepository eventUserRepository,
            ICommentRepository commentRepository,
            IEventRepository eventRepository)
        {
            this.currentUserServices = currentUserServices;
            this.eventUserRepository = eventUserRepository;
            this.commentRepository = commentRepository;
            this.eventRepository = eventRepository;
        }
        public async Task<ServiceResponse<string>> Handle(CreateCommentForEventCommand request, CancellationToken cancellationToken)
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


            var eventDetails = await eventRepository.GetAsync(e => e.Id == request.EventId);
            if (eventDetails == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "Event not found"
                };
            }
            if(eventDetails.EndDate !>= DateTime.UtcNow)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "Event has not ended yet",
                };
            }

            var commentExists = await commentRepository.GetCommentByEventAndUserIdAsync(request.EventId, userId.Value);

            if (commentExists != null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "User has already commented on this event"
                };
            }
            var eventEntity = await eventUserRepository.GetEventoinUserIdAsync(request.EventId);

            if (eventEntity == null || eventEntity.UserId != userId || eventEntity.IsJoined == false)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "User has not joined the event"
                };
            }

            var comment = new Domain.Entities.Comment
            {
                EventId = request.EventId,
                UserId = userId.Value,
                ClubId = eventDetails.ClubId,
                Point = request.Point,
                Description = request.Descripiton,
            };
            await commentRepository.AddAsync(comment);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = "Comment added successfully"
            };
        }
    }
}
