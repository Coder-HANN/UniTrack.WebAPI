using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Club.Query;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetJoinEventCountQueryHandler : IRequestHandler<GetJoinEventCountQuery, ServiceResponse<int>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventUserRepository eventUserRepository;
        private readonly ILocalizationService localizationService;
        public GetJoinEventCountQueryHandler(
            ICurrentUserServices currentUserServices,
            IEventUserRepository eventUserRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.eventUserRepository = eventUserRepository;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<int>> Handle(GetJoinEventCountQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return new ServiceResponse<int>
                {
                    IsSuccess = false,
                    Data = 0,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            var followClubCount = await eventUserRepository.GetJoinEventCountAsync(userId);

            return new ServiceResponse<int>
            {
                IsSuccess = true,
                Data = followClubCount,
                Message = null
            };

        }
    }

}
