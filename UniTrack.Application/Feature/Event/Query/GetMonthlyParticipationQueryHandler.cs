using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetMonthlyParticipationQueryHandler : IRequestHandler<GetMonthlyParticipationQuery, ServiceResponse<List<MonthlyParticipationResponseDTO>>>
    {
        private readonly IEventRepository eventRepository;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;

        public GetMonthlyParticipationQueryHandler(
            IEventRepository eventRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService)
        {
            this.eventRepository = eventRepository;
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<List<MonthlyParticipationResponseDTO>>> Handle(
            GetMonthlyParticipationQuery request,
            CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
                return ServiceResponse<List<MonthlyParticipationResponseDTO>>.Fail(
                    await localizationService.Get(ValidationKeys.NotAuthorized));

            // Son 12 ay
            var now = DateTimeOffset.UtcNow;
            var startDate = now.AddMonths(-11).Date;

            var data = await eventRepository.GetMonthlyParticipationAsync(userId.Value, startDate);

            // Tüm 12 ayı doldur (veri olmayan aylar 0 olsun)
            var result = new List<MonthlyParticipationResponseDTO>();
            for (int i = 11; i >= 0; i--)
            {
                var date = now.AddMonths(-i);
                var existing = data.FirstOrDefault(x => x.Month == date.Month && x.Year == date.Year);
                result.Add(new MonthlyParticipationResponseDTO
                {
                    Month = date.Month,
                    Year = date.Year,
                    MonthName = date.ToString("MMM", new System.Globalization.CultureInfo("tr-TR")),
                    Count = existing?.Count ?? 0
                });
            }

            return ServiceResponse<List<MonthlyParticipationResponseDTO>>.Success(null, result);
        }
    }
}