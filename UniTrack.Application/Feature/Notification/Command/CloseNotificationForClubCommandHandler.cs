using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class CloseNotificationForClubCommandHandler : IRequestHandler<CloseNotificationForClubCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUserClubRepository userClubRepository;
        private readonly ILocalizationService localizationService;
        public CloseNotificationForClubCommandHandler(
            ICurrentUserServices currentUserServices,
            IUserClubRepository userClubRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.userClubRepository = userClubRepository;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<string>> Handle(CloseNotificationForClubCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
           
            if (userId == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized),
                    Data = null
                };
            }

            var userClub = await userClubRepository.GetUserIdInClubAsync(request.ClubId, userId.Value);

            if (userClub == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.UserMustFollowClub),
                    Data = null
                };
            }
            if (userClub.IsNotification == false)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.NotificationAlreadyClosed),
                    Data = null
                };
            }
            userClub.IsNotification = false;

            await userClubRepository.UpdateAsync(userClub);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = await localizationService.Get(ValidationKeys.NotificationClosedSuccess),
                Data = null
            };
        }
    }
}