using FluentValidation.Resources;
using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Notification;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Event.Command
{
    public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, ServiceResponse<DeleteEventResponseDTO>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventRepository eventRepository;
        private readonly INotificationService notificationService;
        private readonly ILocalizationService localization;

        public DeleteEventCommandHandler(
            ICurrentUserServices currentUserServices,
            IEventRepository eventRepository,
            INotificationService notificationService,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.eventRepository = eventRepository;
            this.notificationService = notificationService;
            this.localization = localizationService;
        }

        public async Task<ServiceResponse<DeleteEventResponseDTO>> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            var clubId = currentUserServices.CurrentClub();
            if (userId == null && clubId == null)
            {
                return ServiceResponse<DeleteEventResponseDTO>.Fail(localization.Get(ValidationKeys.NotAuthorized));
            }

            var role = currentUserServices.Role();

            if (role == null || role == Role.User)
            {
                return ServiceResponse<DeleteEventResponseDTO>.Fail(localization.Get(ValidationKeys.NotAuthorized));
            }

            var existingEvent = eventRepository.GetAsync(e => e.Id == request.EventId);

            if (existingEvent == null)
            {
                return ServiceResponse<DeleteEventResponseDTO>.Fail(localization.Get(ValidationKeys.EventNotFound));
            }

            existingEvent.Result.IsDeleted = true;

            await eventRepository.UpdateAsync(existingEvent.Result);


            await notificationService.ClubIsDeleteEventAsync(clubId.Value,localization.Get(ValidationKeys.EventDeletedNotification,existingEvent.Result.Title));

            return ServiceResponse<DeleteEventResponseDTO>.Success(localization.Get(ValidationKeys.EventDeletedSuccess));
        }
    }
}
