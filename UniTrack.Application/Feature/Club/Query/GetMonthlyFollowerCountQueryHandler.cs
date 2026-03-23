// UniTrack.Application/Feature/Club/Query/GetMonthlyFollowerCountQueryHandler.cs
using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Club;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetMonthlyFollowerCountQueryHandler: IRequestHandler<GetMonthlyFollowerCountQuery, ServiceResponse<List<MonthlyFollowerResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IClubRepository clubRepository;
        private readonly ILocalizationService localization;

        public GetMonthlyFollowerCountQueryHandler(
            ICurrentUserServices currentUserServices,
            IClubRepository clubRepository,
            ILocalizationService localization)
        {
            this.currentUserServices = currentUserServices;
            this.clubRepository = clubRepository;
            this.localization = localization;
        }

        public async Task<ServiceResponse<List<MonthlyFollowerResponseDTO>>> Handle(GetMonthlyFollowerCountQuery request,CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();
            var role = currentUserServices.Role();

            if (clubId == null || role == null || role == Role.User)
                return ServiceResponse<List<MonthlyFollowerResponseDTO>>.Fail(
                    await localization.Get(ValidationKeys.NotAuthorized));

            var monthlyData = await clubRepository.GetMonthlyFollowerCountAsync(clubId.Value);

            return ServiceResponse<List<MonthlyFollowerResponseDTO>>.Success(null, monthlyData);
        }
    }
}