using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Notification;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Notification.Query
{
    public class PreviewTargetNotificationQueryHandler : IRequestHandler<PreviewTargetNotificationQuery,ServiceResponse<PreviewTargetNotificationResponseDTO>>
    {
        private readonly IClubRepository clubRepository;
        private readonly IUserRepository userRepository;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;

        public PreviewTargetNotificationQueryHandler(
            IClubRepository clubRepository,
            IUserRepository userRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService)
        {
            this.clubRepository = clubRepository;
            this.userRepository = userRepository;
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<PreviewTargetNotificationResponseDTO>> Handle(PreviewTargetNotificationQuery request,CancellationToken cancellationToken)
        {
            // 🔐 AUTH
            var adminId = currentUserServices.CurrentUser();
            var role = currentUserServices.Role();

            if (adminId == null || role != Role.Admin)
            {
                return ServiceResponse<PreviewTargetNotificationResponseDTO>.Fail(
                    await localizationService.Get(ValidationKeys.NotAuthorized));
            }

            // 🔎 FİLTRE VAR MI?
            bool hasAnyFilter =
                (request.CityIds != null && request.CityIds.Any()) ||
                (request.UniversityIds != null && request.UniversityIds.Any()) ||
                (request.ClubIds != null && request.ClubIds.Any());

            List<Guid> clubIds;

            if (!hasAnyFilter)
            {
                // 🚀 HİÇ FİLTRE YOK → TÜM KULÜPLER
                clubIds = await clubRepository.GetAllClubIdsAsync();
            }
            else
            {
                // 🎯 FİLTRELİ KULÜPLER
                clubIds = await clubRepository.GetFilteredClubIdsAsync(
                    request.CityIds,
                    request.UniversityIds,
                    request.ClubIds
                );
            }

            // ❌ EŞLEŞEN KULÜP YOK
            if (!clubIds.Any())
            {
                return ServiceResponse<PreviewTargetNotificationResponseDTO>.Success(
                    null,
                    new PreviewTargetNotificationResponseDTO
                    {
                        ClubCount = 0,
                        UserCount = 0
                    });
            }

            // 👥 USER COUNT
            var userCount = await userRepository.CountUsersByClubIdsAsync(
                clubIds,
                request.DepartmentIds
            );

            return ServiceResponse<PreviewTargetNotificationResponseDTO>.Success(
                null,
                new PreviewTargetNotificationResponseDTO
                {
                    ClubCount = clubIds.Count,
                    UserCount = userCount
                });
        }
    }
}
