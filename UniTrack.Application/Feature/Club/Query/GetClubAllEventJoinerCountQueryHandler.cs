using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetClubAllEventJoinerCountQueryHandler : IRequestHandler<GetClubAllEventJoinerCountQuery, ServiceResponse<int>>
    {
        private readonly IEventUserRepository eventUserRepository;
        private readonly ICurrentUserServices currentUserService;
        private readonly ILocalizationService localizationService;
        public GetClubAllEventJoinerCountQueryHandler(IEventUserRepository eventUserRepository, ICurrentUserServices currentUserService, ILocalizationService localizationService)
        {
            this.eventUserRepository = eventUserRepository;
            this.currentUserService = currentUserService;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<int>> Handle(GetClubAllEventJoinerCountQuery request, CancellationToken cancellationToken)
        {
            var clubId = currentUserService.CurrentClub();

            if(clubId == null || clubId != request.ClubId)
            {
                return new ServiceResponse<int>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized),
                    Data = 0
                };
            }

            var joinerCount = await eventUserRepository.GetTotalJoinerCountByClubIdAsync(request.ClubId);

            if (joinerCount == 0 || joinerCount == null)
            { 
                return new ServiceResponse<int>
                {
                    IsSuccess = false,
                    Message = null,
                    Data = 0
                };
            }

            return new ServiceResponse<int>
            {
                IsSuccess = true,
                Message = null,
                Data = joinerCount
            };
        }
    }
}
