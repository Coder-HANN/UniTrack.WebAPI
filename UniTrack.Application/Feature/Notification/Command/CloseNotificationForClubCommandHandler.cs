using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class CloseNotificationForClubCommandHandler : IRequestHandler<CloseNotificationForClubCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUserClubRepository userClubRepository;
        public CloseNotificationForClubCommandHandler(
            ICurrentUserServices currentUserServices,
            IUserClubRepository userClubRepository)
        {
            this.currentUserServices = currentUserServices;
            this.userClubRepository = userClubRepository;
        }
        public async Task<ServiceResponse<string>> Handle(CloseNotificationForClubCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
           
            if (userId == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "User not found",
                    Data = null
                };
            }

            var userClub = await userClubRepository.GetUserIdInClubAsync(request.ClubId, userId.Value);

            if (userClub == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "Önce kulübü talip ediniz.",
                    Data = null
                };
            }
            if (userClub.IsNotification == false)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "Notification already closed for this club",
                    Data = null
                };
            }
            userClub.IsNotification = false;

            await userClubRepository.UpdateAsync(userClub);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = "Notification closed for the club successfully",
                Data = null
            };
        }
    }
}
