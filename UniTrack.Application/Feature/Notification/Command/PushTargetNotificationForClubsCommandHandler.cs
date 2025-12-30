using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Entities;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class PushTargetNotificationForClubsCommandHandler: IRequestHandler<PushTargetNotificationForClubsCommand, ServiceResponse<bool>>
    {
        private readonly INotificationRepository notificationRepository;
        private readonly ITargetNotificationRepository targetNotificationRepository;
        private readonly ITargetNotificationCityRepository targetNotificationCityRepository;
        private readonly ITargetNotificationUniversityRepository targetNotificationUniversityRepository;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;

        public PushTargetNotificationForClubsCommandHandler(
            INotificationRepository notificationRepository,
            ITargetNotificationRepository targetNotificationRepository,
            ITargetNotificationCityRepository targetNotificationCityRepository,
            ITargetNotificationUniversityRepository targetNotificationUniversityRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService)
        {
            this.notificationRepository = notificationRepository;
            this.targetNotificationRepository = targetNotificationRepository;
            this.targetNotificationCityRepository = targetNotificationCityRepository;
            this.targetNotificationUniversityRepository = targetNotificationUniversityRepository;
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<bool>> Handle(PushTargetNotificationForClubsCommand request,CancellationToken cancellationToken)
        {
            var adminId = currentUserServices.CurrentUser();
            if (adminId == null)
            {
                return ServiceResponse<bool>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));
            }

            var role = currentUserServices.Role();

            if (role != Domain.Enums.Role.Admin)
            {
                return ServiceResponse<bool>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));
            }

            // 1️⃣ Notification
            var notification = new Domain.Entities.Notification
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Message = request.Message,
                Type = request.Type,
                RelatedEntityId = request.RelatedEntityId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await notificationRepository.AddAsync(notification);

            // 2️⃣ TargetNotification
            var target = new TargetNotification
            {
                Id = Guid.NewGuid(),
                NotificationId = notification.Id
            };

            await targetNotificationRepository.AddAsync(target);

            // 3️⃣ City
            if (request.CityId.HasValue)
            {
                await targetNotificationCityRepository.AddAsync(
                    new TargetNotificationCity
                    {
                        Id = Guid.NewGuid(),
                        TargetNotificationId = target.Id,
                        CityId = request.CityId.Value
                    });
            }

            // 4️⃣ University
            if (request.UniversityId.HasValue)
            {
                await targetNotificationUniversityRepository.AddAsync(
                    new TargetNotificationUniversity
                    {
                        Id = Guid.NewGuid(),
                        TargetNotificationId = target.Id,
                        UniversityId = request.UniversityId.Value
                    });
            }

            return ServiceResponse<bool>.Success(null,true);
        }
    }
}
