using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.ActiveDetail.Query
{
    public class GetActiveClubsFor90DaysQueryHandler : IRequestHandler<GetActiveClubsFor90DaysQuery, ServiceResponse<long>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IClubRepository clubRepository;

        public GetActiveClubsFor90DaysQueryHandler(
            ICurrentUserServices currentUserServices,
            IClubRepository clubRepository)
        {
            this.currentUserServices = currentUserServices;
            this.clubRepository = clubRepository;
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
                    Message = "Yetkisiz kullanıcı"
                };
            }

            var activeClubs = await clubRepository.Get90DaysActiveClubsCountAsync();

            if (activeClubs <= 0)
            {
                return new ServiceResponse<long>
                {
                    IsSuccess = true,
                    Data = 0,
                    Message = "Son 90 günde aktif kulüp bulunamadı."
                };
            }

            return new ServiceResponse<long>
            {
                IsSuccess = true,
                Data = activeClubs,
                Message = $"Son 90 günde {activeClubs} aktif kulüp bulundu."
            };
        }
    }
}