using FluentValidation.Results;
using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Command
{
    public class GetEventDetailCommandHandler : IRequestHandler<GetEventDetailCommand,ServiceResponse<GetEventDetailResponseDTO>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventRepository eventRepository;
        private readonly IEventUserRepository eventUserRepository;
        private readonly ILocalizationService localizationService;
        public GetEventDetailCommandHandler(
            ICurrentUserServices currentUserServices,
            IEventRepository eventRepository,
            IEventUserRepository eventUserRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.eventRepository = eventRepository;
            this.eventUserRepository = eventUserRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<GetEventDetailResponseDTO>> Handle(GetEventDetailCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();

            var eventDetails = await eventRepository.GetEventDetailByIdAsync(request.EventId);
            if (eventDetails == null)
            {
                return ServiceResponse<GetEventDetailResponseDTO>.Fail(ValidationKeys.EventNotFound);
            }
            var rate = eventDetails.EventUsers.Count > 0 ? eventDetails.EventUsers.Average(eu => eu.Event.Quota) : 0;

            var response = new GetEventDetailResponseDTO
            {
                Id = eventDetails.Id,
                Title = eventDetails.Title,
                Description = eventDetails.Description,
                StartDate = eventDetails.StartDate,
                EndDate = eventDetails.EndDate,
                Location = eventDetails.Location,
                ClubId = eventDetails.ClubId,
                ClubName = eventDetails.Club.Name,
                ImageUrls = eventDetails.Images?
                    .OrderBy(i => i.Order)
                    .Select(i => i.ImageUrl)
                    .ToArray() ?? Array.Empty<string>(),
                IsJoined = eventDetails.EventUsers.Any(eu => eu.UserId == userId),
                EventTag = eventDetails.EventTag,
                Status = eventDetails.Status,
                ContectMail = eventDetails.Club.ContectEmail,
                Quota = eventDetails.Quota,
                JoinedCount = eventDetails.EventUsers.Count,
                Rate = rate
            };

            return ServiceResponse<GetEventDetailResponseDTO>.Success(null,response);


        }
    }
}
