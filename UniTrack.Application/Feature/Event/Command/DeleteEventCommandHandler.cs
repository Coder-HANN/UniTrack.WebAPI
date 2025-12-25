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
            var clubId = currentUserServices.CurrentClub();
            if (clubId == null)
            {
                return ServiceResponse<DeleteEventResponseDTO>.Fail(await localization.Get(ValidationKeys.NotAuthorized));
            }

            var role = currentUserServices.Role();

            if (role == null || role == Role.User)
            {
                return ServiceResponse<DeleteEventResponseDTO>.Fail(await localization.Get(ValidationKeys.NotAuthorized));
            }

            var existingEvent = await eventRepository.GetAsync(e => e.Id == request.EventId);

            if (existingEvent == null)
            {
                return ServiceResponse<DeleteEventResponseDTO>.Fail(await localization.Get(ValidationKeys.EventNotFound));
            }

            existingEvent.IsDeleted = true;
            existingEvent.IsActived = false;

            await eventRepository.UpdateAsync(existingEvent);


            await notificationService.ClubIsDeleteEventAsync(clubId.Value, request.EventId,NotificationType.EventDeleted.ToString());

            return ServiceResponse<DeleteEventResponseDTO>.Success(await localization.Get(ValidationKeys.EventDeletedSuccess));
        }
    }
}
