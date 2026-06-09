using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Comment.Command
{
    public class CreateCommentForEventCommandHandler: IRequestHandler<CreateCommentForEventCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventUserRepository eventUserRepository;
        private readonly ICommentRepository commentRepository;
        private readonly IEventRepository eventRepository;
        private readonly ILocalizationService localizationService;

        public CreateCommentForEventCommandHandler(
            ICurrentUserServices currentUserServices,
            IEventUserRepository eventUserRepository,
            ICommentRepository commentRepository,
            IEventRepository eventRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.eventUserRepository = eventUserRepository;
            this.commentRepository = commentRepository;
            this.eventRepository = eventRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(CreateCommentForEventCommand request,CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            var role = currentUserServices.Role();

            if (userId == null)
                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));

            if (role == null || role == Role.Club)

                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));

            var eventDetails = await eventRepository.GetAsync(e => e.Id == request.EventId);

            if (eventDetails == null)

                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.EventNotFound));

            if (eventDetails.EndDate > DateTime.UtcNow)

                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.EventNotFinished));

            var existingComment =await commentRepository.GetCommentByEventAndUserIdAsync(request.EventId, userId.Value);

            if (existingComment != null)

                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.AlreadyCommented));

            var eventUser = await eventUserRepository.GetAsync(

                eu => eu.EventId == request.EventId
                   && eu.UserId == userId.Value
                   && eu.IsJoined == true
                   && eu.IsCheckedIn == true);
                // TO DO: Null geliyo bakılacak.

            if (eventUser == null)
                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.UserNotJoinedEvent));

            var comment = new Domain.Entities.Comment
            {
                EventId = request.EventId,
                UserId = userId.Value,
                ClubId = eventDetails.ClubId,
                Point = request.Point,
                Description = request.Descripiton
            };

            await commentRepository.AddAsync(comment);

            return ServiceResponse<string>.Success(await localizationService.Get(ValidationKeys.CommentCreatedSuccess));
        }
    }
}
