using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetClubMonthlyEventCountQueryHandler : IRequestHandler<GetClubMonthlyEventCountQuery, ServiceResponse<List<MonthlyParticipationResponseDTO>>>
    {
        private readonly IEventRepository eventRepository;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;

        public GetClubMonthlyEventCountQueryHandler(
            IEventRepository eventRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService)
        {
            this.eventRepository = eventRepository;
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<List<MonthlyParticipationResponseDTO>>> Handle(
            GetClubMonthlyEventCountQuery request,
            CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();
            if (clubId == null)
                return ServiceResponse<List<MonthlyParticipationResponseDTO>>.Fail(
                    await localizationService.Get(ValidationKeys.NotAuthorized));

            var now = DateTimeOffset.UtcNow;
            var startDate = now.AddMonths(-11).Date; // Son 12 ay

            var events = await eventRepository.GetCompletedEventsByClubIdAndDateRangeAsync(
                clubId.Value, startDate, now);

            var result = events
                .GroupBy(e => new { e.EndDate.Year, e.EndDate.Month })
                .Select(g => new MonthlyParticipationResponseDTO
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = new DateTime(g.Key.Year, g.Key.Month, 1)
                                    .ToString("MMM", new System.Globalization.CultureInfo("tr-TR")),
                    Count = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            // Boş ayları da doldur
            var filled = new List<MonthlyParticipationResponseDTO>();
            for (int i = 11; i >= 0; i--)
            {
                var date = now.AddMonths(-i);
                var existing = result.FirstOrDefault(x => x.Year == date.Year && x.Month == date.Month);
                filled.Add(existing ?? new MonthlyParticipationResponseDTO
                {
                    Year = date.Year,
                    Month = date.Month,
                    MonthName = new DateTime(date.Year, date.Month, 1)
                                    .ToString("MMM", new System.Globalization.CultureInfo("tr-TR")),
                    Count = 0
                });
            }

            return ServiceResponse<List<MonthlyParticipationResponseDTO>>.Success(null, filled);
        }
    }
}