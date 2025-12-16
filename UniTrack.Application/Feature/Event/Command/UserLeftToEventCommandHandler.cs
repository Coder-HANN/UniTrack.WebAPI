using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Sheets;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Event.Command
{
    public class UserLeftToEventCommandHandler
        : IRequestHandler<UserLeftToEventCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventUserRepository eventUserRepository;
        private readonly IEventRepository eventRepository;
        private readonly IParticipantSheetRepository participantSheetRepository;
        private readonly ILocalizationService localizationService;

        public UserLeftToEventCommandHandler(
            ICurrentUserServices currentUserServices,
            IEventUserRepository eventUserRepository,
            IEventRepository eventRepository,
            IParticipantSheetRepository participantSheetRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.eventUserRepository = eventUserRepository;
            this.eventRepository = eventRepository;
            this.participantSheetRepository = participantSheetRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(UserLeftToEventCommand request,CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));
            }

            var role = currentUserServices.Role();
            if (role == null || role == Role.Club)
            {
                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));
            }

            var eventUser = await eventUserRepository.GetAsync(
                eu => eu.EventId == request.EventId
                   && eu.UserId == userId
                   && eu.IsJoined);

            if (eventUser == null)
            {
                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.NotJoinedEvent));
            }

            // DB - ilişki sil
            await eventUserRepository.DeleteAsync(eventUser);

            var eventEntity = await eventRepository.GetAsync(e => e.Id == request.EventId);
            if (eventEntity != null)
            {
                eventEntity.Joiner = Math.Max(0, eventEntity.Joiner - 1);
                await eventRepository.UpdateAsync(eventEntity);
            }

            // GOOGLE SHEETS DELETE
            var sheetId = eventEntity?.SheetsId;
            var userEmail = eventUser.User?.Email;

            if (!string.IsNullOrEmpty(sheetId) && !string.IsNullOrEmpty(userEmail))
            {
                try
                {
                    await participantSheetRepository.RemoveParticipantAsync(sheetId, userEmail);
                }
                catch
                {
                    // loglanır, user etkilenmez
                }
            }

            return ServiceResponse<string>.Success(await localizationService.Get(ValidationKeys.EventLeftSuccess));
        }
    }
}
