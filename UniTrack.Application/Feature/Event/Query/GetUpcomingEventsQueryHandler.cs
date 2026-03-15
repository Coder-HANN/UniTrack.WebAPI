// Handler
using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetUpcomingEventsQueryHandler : IRequestHandler<GetUpcomingEventsQuery, ServiceResponse<List<UpcomingEventResponseDTO>>>
    {
        private readonly IEventRepository eventRepository;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;

        public GetUpcomingEventsQueryHandler(
            IEventRepository eventRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService)
        {
            this.eventRepository = eventRepository;
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<List<UpcomingEventResponseDTO>>> Handle(
            GetUpcomingEventsQuery request,
            CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
                return ServiceResponse<List<UpcomingEventResponseDTO>>.Fail(
                    await localizationService.Get(ValidationKeys.NotAuthorized));

            var now = DateTimeOffset.UtcNow;
            const int maxCount = 5;
            var result = new List<UpcomingEventResponseDTO>();

            // 1️⃣ Dopingli etkinlikler — max 5
            var dopingEvents = await eventRepository.GetUpcomingDopingEventsAsync(now, maxCount);
            result.AddRange(dopingEvents.Select(Map));

            if (result.Count >= maxCount)
                return ServiceResponse<List<UpcomingEventResponseDTO>>.Success(null, result);

            // 2️⃣ Kullanıcının katıldığı yaklaşan etkinlikler
            var remaining = maxCount - result.Count;
            var dopingIds = result.Select(x => x.EventId).ToHashSet();

            var joinedEvents = await eventRepository.GetUpcomingJoinedEventsAsync(
                userId.Value, now, remaining, dopingIds);
            result.AddRange(joinedEvents.Select(Map));

            if (result.Count >= maxCount)
                return ServiceResponse<List<UpcomingEventResponseDTO>>.Success(null, result);

            // 3️⃣ Genel yaklaşan etkinlikler (doping + joined hariç)
            remaining = maxCount - result.Count;
            var excludedIds = result.Select(x => x.EventId).ToHashSet();

            var generalEvents = await eventRepository.GetUpcomingGeneralEventsAsync(
                now, remaining, excludedIds);
            result.AddRange(generalEvents.Select(Map));

            return ServiceResponse<List<UpcomingEventResponseDTO>>.Success(null, result);
        }

        private static UpcomingEventResponseDTO Map(Domain.Entities.Event e) =>
            new()
            {
                EventId = e.Id,
                Title = e.Title,
                StartDate = e.StartDate,
                ClubName = e.Club?.Name ?? "-",
                IsDoping = e.IsDoping
            };
    }
}