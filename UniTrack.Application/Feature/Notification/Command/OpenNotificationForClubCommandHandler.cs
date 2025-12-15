using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class OpenNotificationForClubCommandHandler : IRequestHandler<OpenNotificationForClubCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUserClubRepository userClubRepository;
        public OpenNotificationForClubCommandHandler(
            ICurrentUserServices currentUserServices,
            IUserClubRepository userClubRepository)
        {
            this.currentUserServices = currentUserServices;
            this.userClubRepository = userClubRepository;
        }
        public async  Task<ServiceResponse<string>> Handle(OpenNotificationForClubCommand request, CancellationToken cancellationToken)
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
            if(userClub == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "Önce kulübü talip ediniz.",
                    Data = null
                };
            }

            if (userClub.IsNotification == true)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "Notification already opened for this club",
                    Data = null
                };
            }

            userClub.IsNotification = true;
            
            await userClubRepository.UpdateAsync(userClub);
            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = "Notification opened for the club successfully",
                Data = null
            };

        }
    }
}
