using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.ActiveDetail.Query
{
    public class GetActiveClubsFor360DaysQueryHandler : IRequestHandler<GetActiveClubsFor360DaysQuery, ServiceResponse<long>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IClubRepository clubRepository;

        public GetActiveClubsFor360DaysQueryHandler(
            ICurrentUserServices currentUserServices,
            IClubRepository clubRepository)
        {
            this.currentUserServices = currentUserServices;
            this.clubRepository = clubRepository;
        }

        public async Task<ServiceResponse<long>> Handle(GetActiveClubsFor360DaysQuery request, CancellationToken cancellationToken)
        {
            var role = currentUserServices.Role();
            if (role == null || role != Role.Admin)
            {
                return new ServiceResponse<long>
                {
                    IsSuccess = false,
                    Data = 0,
                    Message = "Yetkisiz kullanıcı. Yalnızca Admin rolüne sahip kullanıcılar bu veriyi sorgulayabilir."
                };
            }

            var activeClubs = await clubRepository.Get360DaysActiveClubsCountAsync();

            if (activeClubs <= 0)
            {
                return new ServiceResponse<long>
                {
                    IsSuccess = true,
                    Data = 0,
                    Message = "Son 360 günde aktif kulüp bulunamadı."
                };
            }

            return new ServiceResponse<long>
            {
                IsSuccess = true,
                Data = activeClubs,
                Message = $"Son 360 günde {activeClubs} aktif kulüp bulundu."
            };
        }
    }
}