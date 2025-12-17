using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetFollowClubCountQueryHandler : IRequestHandler<GetFollowClubCountQuery, ServiceResponse<int>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUserClubRepository userClubRepository;
        private readonly ILocalizationService localizationService;
        public GetFollowClubCountQueryHandler(
            ICurrentUserServices currentUserServices,
            IUserClubRepository userClubRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.userClubRepository = userClubRepository;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<int>> Handle(GetFollowClubCountQuery request, CancellationToken cancellationToken)
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

            var followClubCount = await userClubRepository.GetFollowedClubCountAsync(userId);

            return new ServiceResponse<int>
            {
                IsSuccess = true,
                Data =  followClubCount,
                Message = null
            };

        }
    }
}
