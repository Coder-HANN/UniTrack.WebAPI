using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetCalendarEventsQueryHandler : IRequestHandler<GetCalendarEventsQuery, ServiceResponse<List<CalendarEventResponseDTO>>>
    {
        private readonly IEventRepository eventRepository;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;

        public GetCalendarEventsQueryHandler(
            IEventRepository eventRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService)
        {
            this.eventRepository = eventRepository;
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
        }

        // Handler'da değişiklik
        public async Task<ServiceResponse<List<CalendarEventResponseDTO>>> Handle(GetCalendarEventsQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
                return ServiceResponse<List<CalendarEventResponseDTO>>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));

            // ✅ DB'ye gitmeden claim'den al
            var universityId = currentUserServices.UniversityId();
            if (universityId == null)
                return ServiceResponse<List<CalendarEventResponseDTO>>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));

            var events = await eventRepository.GetCalendarEventsByUserUniversityAsync(universityId.Value);

            var result = events.Select(Map).ToList();

            return ServiceResponse<List<CalendarEventResponseDTO>>.Success(null, result);
        }
        private static CalendarEventResponseDTO Map(Domain.Entities.Event e) => new()
        {
            EventId = e.Id,
            Title = e.Title,
            StartDate = e.StartDate.ToString("O"),
            ClubName = e.Club?.Name ?? "-",
            ClubLogoUrl = e.Club?.LogoUrl,
            IsDoping = e.IsDoping,
            Location = e.Location,
            Time = e.StartDate.ToString("HH:mm")
        };
    }
}