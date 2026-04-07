using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetClubEventReportQueryHandler: IRequestHandler<GetClubEventReportQuery, ServiceResponse<List<ClubEventReportResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventRepository eventRepository;
        private readonly ILocalizationService localization;

        public GetClubEventReportQueryHandler(
            ICurrentUserServices currentUserServices,
            IEventRepository eventRepository,
            ILocalizationService localization)
        {
            this.currentUserServices = currentUserServices;
            this.eventRepository = eventRepository;
            this.localization = localization;
        }

        public async Task<ServiceResponse<List<ClubEventReportResponseDTO>>> Handle(GetClubEventReportQuery request,CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();
            var role = currentUserServices.Role();

            if (clubId == null || role == null || role == Role.User)
                return ServiceResponse<List<ClubEventReportResponseDTO>>.Fail(await localization.Get(ValidationKeys.NotAuthorized));
            

            var events = await eventRepository.GetAllByClubIdAsync(clubId.Value);

            var report = events.Select(e => {

                int currentCount = e.EventUsers?.Count(eu => eu.IsCheckedIn) ?? 0;

                return new ClubEventReportResponseDTO
                {
                    EventId = e.Id,
                    Title = e.Title,
                    JoinedCount = currentCount,
                    Quota = e.Quota,
                    FillRate = e.Quota > 0 ? Math.Round(((double)currentCount / e.Quota) * 100, 1) : 0
                };

            }).ToList();

            return ServiceResponse<List<ClubEventReportResponseDTO>>.Success(null,report);
        }
    }
}