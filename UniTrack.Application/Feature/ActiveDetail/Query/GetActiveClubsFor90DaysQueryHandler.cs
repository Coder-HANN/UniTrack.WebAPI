using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.ActiveDetail.Query
{
    public class GetActiveClubsFor90DaysQueryHandler : IRequestHandler<GetActiveClubsFor90DaysQuery, ServiceResponse<long>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IClubRepository clubRepository;
        private readonly ILocalizationService localizationService;

        public GetActiveClubsFor90DaysQueryHandler(
            ICurrentUserServices currentUserServices,
            IClubRepository clubRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.clubRepository = clubRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<long>> Handle(GetActiveClubsFor90DaysQuery request, CancellationToken cancellationToken)
        {
            var role = currentUserServices.Role();
            if (role == null || role != Role.Admin)
            {
                return new ServiceResponse<long>
                {
                    IsSuccess = false,
                    Data = 0,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            var activeClubs = await clubRepository.Get90DaysActiveClubsCountAsync();

            if (activeClubs == 0)
            {
                return new ServiceResponse<long>
                {
                    IsSuccess = true,
                    Data = 0,
                    Message = null
                };
            }

            return new ServiceResponse<long>
            {
                IsSuccess = true,
                Data = activeClubs,
                Message = null
            };
        }
    }
}