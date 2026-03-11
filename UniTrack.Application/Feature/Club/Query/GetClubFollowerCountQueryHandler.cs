using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetClubFollowerCountQueryHandler : IRequestHandler<GetClubFollowerCountQuery, ServiceResponse<long>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IClubRepository clubRepository;
        private readonly ILocalizationService localizationService;
        public GetClubFollowerCountQueryHandler(
            ICurrentUserServices currentUserServices,
            IClubRepository clubRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.clubRepository = clubRepository;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<long>> Handle(GetClubFollowerCountQuery request, CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();

            if (clubId == null)
            {
                return new ServiceResponse<long>
                {
                    IsSuccess = false,
                    Data = 0,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            var followerCount = await clubRepository.GetClubFollowerCountAsync(clubId.Value);
            if (followerCount == 0 || followerCount == null)
            {
                return new ServiceResponse<long>
                {
                    IsSuccess = false,
                    Data = 0,
                    Message = null
                };
            }

            return new ServiceResponse<long>
            {
                IsSuccess = true,
                Data = followerCount,
                Message = null
            };
        }
    }
}
