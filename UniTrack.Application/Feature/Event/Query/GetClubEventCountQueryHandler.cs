using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetClubEventCountQueryHandler : IRequestHandler<GetClubEventCountQuery, ServiceResponse<int>>
    {
        private readonly IEventRepository eventRepository;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;

        public GetClubEventCountQueryHandler(
            IEventRepository eventRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService)
        {
            this.eventRepository = eventRepository;
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<int>> Handle(
            GetClubEventCountQuery request,
            CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();
            if (clubId == null)
                return ServiceResponse<int>.Fail(
                    await localizationService.Get(ValidationKeys.NotAuthorized));

            var now = DateTimeOffset.UtcNow;
            var count = await eventRepository.GetCompletedEventCountByClubIdAsync(clubId.Value, now);

            return ServiceResponse<int>.Success(null, count);
        }
    }
}