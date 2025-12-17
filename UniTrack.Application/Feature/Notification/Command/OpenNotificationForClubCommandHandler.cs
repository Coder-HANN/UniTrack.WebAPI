using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class OpenNotificationForClubCommandHandler : IRequestHandler<OpenNotificationForClubCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUserClubRepository userClubRepository;
        private readonly ILocalizationService localizationService;
        public OpenNotificationForClubCommandHandler(
            ICurrentUserServices currentUserServices,
            IUserClubRepository userClubRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.userClubRepository = userClubRepository;
            this.localizationService = localizationService;
        }
        public async  Task<ServiceResponse<string>> Handle(OpenNotificationForClubCommand request, CancellationToken cancellationToken)
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

            if(userClub == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.UserMustFollowClub),
                    Data = null
                };
            }

            if (userClub.IsNotification == true)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.NotificationAlreadyOpened),
                    Data = null
                };
            }

            userClub.IsNotification = true;
            
            await userClubRepository.UpdateAsync(userClub);
            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = await localizationService.Get(ValidationKeys.NotificationOpenedSuccess),
                Data = null
            };

        }
    }
}
