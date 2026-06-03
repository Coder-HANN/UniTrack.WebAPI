using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Event.Command;

namespace UniTrack.Application.Feature.Event.Command
{
    public class LikeEventCommandHandler : IRequestHandler<LikeEventCommand, ServiceResponse<string>>
    {
        private readonly IEventUserRepository eventUserRepository;
        private readonly IEventRepository eventRepository;
        private readonly ICurrentUserServices currentUser;
        private readonly ILocalizationService localization;

        public LikeEventCommandHandler(
            IEventUserRepository eventUserRepository,
            IEventRepository eventRepository,
            ICurrentUserServices currentUser,
            ILocalizationService localization)
        {
            this.eventUserRepository = eventUserRepository;
            this.eventRepository = eventRepository;
            this.currentUser = currentUser;
            this.localization = localization;
        }

        public async Task<ServiceResponse<string>> Handle(LikeEventCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUser.CurrentUser();
            if (userId == null)
                return ServiceResponse<string>.Fail(await localization.Get(ValidationKeys.NotAuthorized));

            var eventEntity = await eventRepository.GetByIdAsync(request.EventId);
            if (eventEntity is null)
                return ServiceResponse<string>.Fail(await localization.Get(ValidationKeys.EventNotFound));

            var eventUser = await eventUserRepository.GetByEventAndUserAsync(request.EventId, userId.Value);

            if (eventUser is null)
            {
                var newEventUser = new Domain.Entities.EventUser
                {
                    EventId = request.EventId,
                    UserId = userId.Value,
                    IsLiked = true
                };
                await eventUserRepository.AddAsync(newEventUser);
            }
            else
            {
                if (eventUser.IsLiked)
                    return ServiceResponse<string>.Fail("Bu etkinliği zaten beğendiniz.");

                eventUser.IsLiked = true;
                await eventUserRepository.UpdateAsync(eventUser);
            }

            return ServiceResponse<string>.Success(null, "Etkinlik başarıyla beğenildi.");
        }
    }
}