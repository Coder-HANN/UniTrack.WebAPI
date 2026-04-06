using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Sheets;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Event.Command
{
    public class UserJoinToEventCommandHandler : IRequestHandler<UserJoinToEventCommand, ServiceResponse<UserJoinToEventResponseDTO>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventUserRepository eventUserRepository;
        private readonly IEventRepository eventRepository;
        private readonly IUserDetailRepository userDetailRepository;
        private readonly IParticipantSheetRepository participantSheetRepository;
        private readonly ILocalizationService localizationService;
        public UserJoinToEventCommandHandler(
            ICurrentUserServices currentUserServices,
            IEventUserRepository eventUserRepository,
            IEventRepository eventRepository,
            IUserDetailRepository userDetailRepository,
            IParticipantSheetRepository participantSheetRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.eventUserRepository = eventUserRepository;
            this.eventRepository = eventRepository;
            this.userDetailRepository = userDetailRepository;
            this.participantSheetRepository = participantSheetRepository;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<UserJoinToEventResponseDTO>> Handle(UserJoinToEventCommand request,CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();

            if (userId == null || currentUserServices.Role() != Role.User)
            {
                return ServiceResponse<UserJoinToEventResponseDTO>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));
            }

            var eventEntity = await eventRepository.GetEventIdAsync(request.EventId);

            if (eventEntity == null)
            {
                return ServiceResponse<UserJoinToEventResponseDTO>.Fail(await localizationService.Get(ValidationKeys.EventNotFound));
            }

            if (eventEntity.EndDate < DateTime.UtcNow)
            {
                return ServiceResponse<UserJoinToEventResponseDTO>.Fail(await localizationService.Get(ValidationKeys.EventExpired)
                );
            }

            var alreadyJoined = await eventUserRepository.GetAsync(eu => eu.EventId == request.EventId && eu.UserId == userId.Value && eu.IsJoined == true);

            if (alreadyJoined != null)
            {
                return ServiceResponse<UserJoinToEventResponseDTO>.Fail(await localizationService.Get(ValidationKeys.AlreadyJoinedEvent)
                );
            }

            if (eventEntity.Joiner >= eventEntity.Quota)
            {
                return ServiceResponse<UserJoinToEventResponseDTO>.Fail(await localizationService.Get(ValidationKeys.EventQuotaFull)
                );
            }


            var userDetail = await userDetailRepository.GetUserForJoinAsync(userId.Value);

            if (eventEntity.Status != Status.Public && eventEntity.Club.UniversityId != userDetail.UniverstiyId)
            {
                return ServiceResponse<UserJoinToEventResponseDTO>.Fail(
                    await localizationService.Get(ValidationKeys.EventUniversityOnly)
                );
            }

            // === CORE BUSINESS ===
            await eventUserRepository.AddAsync(new EventUser
            {
                EventId = request.EventId,
                UserId = userId.Value,
                IsJoined = true,
                IsCheckedIn = false,
            });

            var joinerCountAdded = await eventRepository.CountaddedAsync(request.EventId);

            await eventRepository.UpdateAsync(eventEntity);



            // === SIDE EFFECT (Sheets) ===
            if (!string.IsNullOrEmpty(eventEntity.SheetsId))
            {
                try
                {
                    var participant = new SheetParticipantDTO
                    {
                        Email = userDetail.User?.Email,
                        Name = userDetail.Name,
                        Surname = userDetail.Surname,
                        UniversityName = userDetail.University?.Name,
                        DepartmentName = userDetail.Department?.Name,
                        Graduaiton_Date = userDetail.Graduaiton_Date.ToString("dd.MM.yyyy"),
                        JoinDate = DateTimeOffset.UtcNow
                    };

                    await participantSheetRepository.AddParticipantAsync(eventEntity.SheetsId,participant);
                }
                catch (Exception ex)
                {
                    // sadece loglanır
                }
            }

            return ServiceResponse<UserJoinToEventResponseDTO>.Success(await localizationService.Get(ValidationKeys.EventJoinSuccess)
            );
        }

    }
}
