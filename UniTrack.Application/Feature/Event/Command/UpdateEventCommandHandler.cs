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
    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, ServiceResponse<UpdateEventResponseDTO>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventRepository eventRepository;
        private readonly INotificationService notificationService;
        private readonly ILocalizationService localizationService;

        public UpdateEventCommandHandler(
            ICurrentUserServices currentUserServices,
            IEventRepository eventRepository,
            INotificationService notificationService,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.eventRepository = eventRepository;
            this.notificationService = notificationService;
            this.localizationService = localizationService;
        }
        
        public async Task<ServiceResponse<UpdateEventResponseDTO>> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();

            if (clubId == null)
            {
                return new ServiceResponse<UpdateEventResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }
            var role = currentUserServices.Role();
            if (role == null  || role == Role.User)
            {
                return new ServiceResponse<UpdateEventResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            var existingEvent = await eventRepository.GetByIdAsync(request.Id);

            if (existingEvent == null)
            {
                return new ServiceResponse<UpdateEventResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.EventNotFound)
                };
            }

            bool isUpdated = false;

            if (!string.IsNullOrEmpty(request.Title) && existingEvent.Title != request.Title)
            {
                existingEvent.Title = request.Title;
                isUpdated = true;
            }

            if (!string.IsNullOrEmpty(request.Description) && existingEvent.Description != request.Description)
            {
                existingEvent.Description = request.Description;
                isUpdated = true;
            }

            if (request.StartDate != default && existingEvent.StartDate != request.StartDate)
            {
                existingEvent.StartDate = request.StartDate;
                isUpdated = true;
            }

            if (request.EndDate != default && existingEvent.EndDate != request.EndDate)
            {
                existingEvent.EndDate = request.EndDate;
                isUpdated = true;
            }

            // null değilse ve içerisinde eleman varsa
            if (request.ImageUrl != null && request.ImageUrl.Any())
            {
                // Mevcut resimlerle yeni gelen resimler aynı mı? (Sıralama dahil kontrol eder)
                if (existingEvent.ImageUrl == null || !existingEvent.ImageUrl.SequenceEqual(request.ImageUrl))
                {
                    existingEvent.ImageUrl = request.ImageUrl;
                    isUpdated = true;
                }
            }


            if (request.Quota > 0 && existingEvent.Quota != request.Quota)
            {
                existingEvent.Quota = request.Quota;
                isUpdated = true;
            }

            if (request.EventTag != default && existingEvent.EventTag != request.EventTag)
            {
                existingEvent.EventTag = request.EventTag;
                isUpdated = true;
            }


            if (!string.IsNullOrEmpty(request.Location) && existingEvent.Location != request.Location)
            {
                existingEvent.Location = request.Location;
                isUpdated = true;
            }

            if (request.Clock != default && existingEvent.Clock != request.Clock)
            {
                existingEvent.Clock = request.Clock;
                isUpdated = true;
            }

            if (request.Status != existingEvent.Status)
            {
                existingEvent.Status = request.Status;
                isUpdated = true;
            }

            if (!isUpdated)
            {
                return ServiceResponse<UpdateEventResponseDTO>.Success(
                    await localizationService.Get(ValidationKeys.EventNotModified)
                );
            }
                await eventRepository.UpdateAsync(existingEvent);

                var message = await localizationService.Get(ValidationKeys.EventUpdatedNotification,existingEvent.Title);

                await notificationService.ClubIsUpdateEventAsync(existingEvent.ClubId, existingEvent.Id, message);

            return ServiceResponse<UpdateEventResponseDTO>.Success(await localizationService.Get(ValidationKeys.EventUpdatedSuccess));
        }
    }
}
