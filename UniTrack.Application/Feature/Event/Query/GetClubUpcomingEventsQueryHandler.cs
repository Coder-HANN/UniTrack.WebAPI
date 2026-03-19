using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetClubUpcomingEventsQueryHandler : IRequestHandler<GetClubUpcomingEventsQuery, ServiceResponse<List<UpcomingEventResponseDTO>>>
    {
        private readonly IEventRepository eventRepository;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;

        public GetClubUpcomingEventsQueryHandler(
            IEventRepository eventRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService)
        {
            this.eventRepository = eventRepository;
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<List<UpcomingEventResponseDTO>>> Handle(GetClubUpcomingEventsQuery request,CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();
            if (clubId == null)
                return ServiceResponse<List<UpcomingEventResponseDTO>>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));

            var now = DateTimeOffset.UtcNow;
            const int maxCount = 5;

            var events = await eventRepository.GetUpcomingEventsByClubIdAsync(clubId.Value, now, maxCount);

            var result = events.Select(Map).ToList();

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