using MediatR;
using System.Reflection;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Notification;
using UniTrack.Domain.Entities;

namespace UniTrack.Application.Feature.Notification.Query
{
    public class GetUserAllNotificationHistoryQueryHandler : IRequest<ServiceResponse<List<GetUserAllNotificationHistoryResponse>>>
    {
        private readonly ILocalizationService localizationService;
        private readonly ICurrentUserServices currentUserServices;
        private readonly INotificationRepository notificationRepository;

        public GetUserAllNotificationHistoryQueryHandler(
            ILocalizationService localizationService,
            ICurrentUserServices currentUserServices,
            INotificationRepository notificationRepository)
        {
            this.localizationService = localizationService;
            this.currentUserServices = currentUserServices;
            this.notificationRepository = notificationRepository;
        }

        public async Task<ServiceResponse<List<GetUserAllNotificationHistoryResponse>>> Handler(GetUserAllNotificationHistoryQuery query , CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            var clubId = currentUserServices.CurrentClub();

            if (userId == null && clubId == null)
            {
                return new ServiceResponse<List<GetUserAllNotificationHistoryResponse>> 
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            List<Domain.Entities.Notification> notifications;

            if (userId != null)
            {
                notifications = await notificationRepository
                    .GetUserAllNotification(userId.Value);
            }
            else
            {
                notifications = await notificationRepository
                    .GetClubAllNotification(clubId!.Value);
            }

            if (!notifications.Any())
            {
                return new ServiceResponse<List<GetUserAllNotificationHistoryResponse>>
                {
                    IsSuccess = true,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.NotificationNotFound)
                };
            }
            var responses = notifications.Select(n => new GetUserAllNotificationHistoryResponse
            {
                Title = n.Title,
                Message = n.Message,
                Logo = n.Logo

            }).ToList();

            return new ServiceResponse<List<GetUserAllNotificationHistoryResponse>>
            {
                IsSuccess = true,
                Data = responses,
                Message = null
            };
        }
    }
}
