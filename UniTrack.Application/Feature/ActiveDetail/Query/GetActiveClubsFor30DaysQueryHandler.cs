using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.ActiveDetail.Query
{
    public class GetActiveClubsFor30DaysQueryHandler : IRequestHandler<GetActiveClubsFor30DaysQuery, ServiceResponse<long>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IClubRepository clubRepository; 
        private readonly ILocalizationService localization;

        public GetActiveClubsFor30DaysQueryHandler(
            ICurrentUserServices currentUserServices,
            IClubRepository clubRepository,
            ILocalizationService localization)
        {
            this.currentUserServices = currentUserServices;
            this.clubRepository = clubRepository;
            this.localization = localization;
        }

        public async Task<ServiceResponse<long>> Handle(GetActiveClubsFor30DaysQuery request, CancellationToken cancellationToken)
        {
            var role = currentUserServices.Role();
            if (role == null || role != Role.Admin)
            {
                return new ServiceResponse<long>
                {
                    IsSuccess = false,
                    Data = 0,
                    Message = await localization.Get(ValidationKeys.NotAuthorized)
                };
            }

            var activeClubs = await clubRepository.Get30DaysActiveClubsCountAsync();

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