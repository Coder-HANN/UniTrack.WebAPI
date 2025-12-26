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
    public class PreviewTargetNotificationQueryHandler: IRequestHandler<PreviewTargetNotificationQuery,ServiceResponse<PreviewTargetNotificationResponse>>
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

        public async Task<ServiceResponse<PreviewTargetNotificationResponse>> Handle(PreviewTargetNotificationQuery request,CancellationToken cancellationToken)
        {
            var adminId = currentUserServices.CurrentUser();
            var role = currentUserServices.Role();

            if (adminId == null || role != Role.Admin)
            {
                return ServiceResponse<PreviewTargetNotificationResponse>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));
            }

            var clubIds = await clubRepository.GetFilteredClubIdsAsync(request.CityIds,request.UniversityIds,request.ClubIds);

            if (!clubIds.Any())
            {
                return ServiceResponse<PreviewTargetNotificationResponse>.Success(null,
                    new PreviewTargetNotificationResponse
                    {
                        ClubCount = 0,
                        UserCount = 0
                    });
            }

            var userCount = await userRepository.CountUsersByClubIdsAsync(clubIds,request.DepartmentIds);

            return ServiceResponse<PreviewTargetNotificationResponse>.Success(null,
                new PreviewTargetNotificationResponse
                {
                    ClubCount = clubIds.Count,
                    UserCount = userCount
                });

        }
    }

}
