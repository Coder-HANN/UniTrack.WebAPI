using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Notification;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Event.Command
{
    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, ServiceResponse<UpdateEventResponseDTO>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventRepository eventRepository;
        private readonly INotificationService notificationService;

        public UpdateEventCommandHandler(
            ICurrentUserServices currentUserServices,
            IEventRepository eventRepository,
            INotificationService notificationService)
        {
            this.currentUserServices = currentUserServices;
            this.eventRepository = eventRepository;
            this.notificationService = notificationService;
        }
        
        public async Task<ServiceResponse<UpdateEventResponseDTO>> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentClub();

            if (userId == null)
            {
                return new ServiceResponse<UpdateEventResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Unauthorized"
                };
            }
            var role = currentUserServices.Role();
            if (role == null  || role == Role.User)
            {
                return new ServiceResponse<UpdateEventResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Yetkisiz kullanıcı"
                };
            }

            var existingEvent = await eventRepository.GetByIdAsync(request.Id);

            if (existingEvent == null)
            {
                return new ServiceResponse<UpdateEventResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Event not found"
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

            if (request.Image != null && !existingEvent.Image.SequenceEqual(request.Image))
            {
                existingEvent.Image = request.Image;
                isUpdated = true;
            }

            if (request.Quota > 0 && existingEvent.Quota != request.Quota)
            {
                existingEvent.Quota = request.Quota;
                isUpdated = true;
            }

            if (!string.IsNullOrEmpty(request.Tag.ToString()) && existingEvent.Tag != request.Tag)
            {
                existingEvent.Tag = request.Tag;
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
          

            if (isUpdated)
            {
                await eventRepository.UpdateAsync(existingEvent);

                await notificationService.ClubIsUpdateEventAsync(existingEvent.ClubId,$"Katılmış olduğunuz '{existingEvent.Title}' etkinliği güncellendi.");

                return new ServiceResponse<UpdateEventResponseDTO>
                {
                    IsSuccess = true,
                    Data = null,
                    Message = "Event updated successfully"
                };

                
                
            }

            

            return null;
        }
    }
}
