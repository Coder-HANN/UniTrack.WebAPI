using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using UniTrack.Persistence.Repositories;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class SendNotificationCommandHandler: IRequestHandler<SendNotificationCommand, ServiceResponse<string>>
    {
        private readonly INotificationRepository notificationRepository;
        private readonly ITargetNotificationRepository targetRepository;
        private readonly ITargetNotificationCityRepository cityRepo;
        private readonly ITargetNotificationUniversityRepository universityRepo;
        private readonly ITargetNotificationDepartmentRepository departmentRepo;
        private readonly ITargetNotificationClubRepository clubRepo;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;

        public SendNotificationCommandHandler(
            INotificationRepository notificationRepository,
            ITargetNotificationRepository targetNotificationRepository,
            ITargetNotificationCityRepository targetNotificationCityRepository,
            ITargetNotificationDepartmentRepository targetNotificationDepartmentRepository,
            ITargetNotificationUniversityRepository targetNotificationUniversityRepository,
            ITargetNotificationClubRepository targetNotificationClubRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService)
        {
            this.notificationRepository = notificationRepository;
            this.targetRepository = targetNotificationRepository;
            this.cityRepo = targetNotificationCityRepository;
            this.universityRepo = targetNotificationUniversityRepository;
            this.departmentRepo = targetNotificationDepartmentRepository;
            this.clubRepo = targetNotificationClubRepository;
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>>Handle(SendNotificationCommand request,CancellationToken cancellationToken)
        {
            var adminId = currentUserServices.CurrentUser();
            var role = currentUserServices.Role();

            if (role != Role.Admin && adminId == null) 
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }
            // 1️⃣ Notification oluştur
            var notification = new Domain.Entities.Notification
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Message = request.Message,
                Type = request.Type,
                RelatedEntityId = request.RelatedEntityId
            };

            await notificationRepository.AddAsync(notification);

            // 2️⃣ TargetNotification oluştur
            var target = new TargetNotification
            {
                Id = Guid.NewGuid(),
                NotificationId = notification.Id
            };

            await targetRepository.AddAsync(target);

            // 3️⃣ Filtreleri yaz
            if (request.CityIds?.Any() == true)
            {
                await cityRepo.AddRangeAsync(
                    request.CityIds.Select(id =>
                        new TargetNotificationCity
                        {
                            Id = Guid.NewGuid(),
                            TargetNotificationId = target.Id,
                            CityId = id
                        }).ToList());
            }

            if (request.UniversityIds?.Any() == true)
            {
                await universityRepo.AddRangeAsync(request.UniversityIds.Select(id =>
                        new TargetNotificationUniversity
                        {
                            Id = Guid.NewGuid(),
                            TargetNotificationId = target.Id,
                            UniversityId = id
                        }).ToList());
            }

            if (request.DepartmentIds?.Any() == true)
            {
                await departmentRepo.AddRangeAsync(request.DepartmentIds.Select(id =>
                        new TargetNotificationDepartment
                        {
                            Id = Guid.NewGuid(),
                            TargetNotificationId = target.Id,
                            DepartmentId = id
                        }).ToList());
            }

            if (request.ClubIds?.Any() == true)
            {
                await clubRepo.AddRangeAsync(request.ClubIds.Select(id =>
                        new TargetNotificationClub
                        {
                            Id = Guid.NewGuid(),
                            TargetNotificationId = target.Id,
                            ClubId = id
                        }).ToList());
            }

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Data = null,
                Message = await localizationService.Get(ValidationKeys.NotificationSendSuccess)
            };
        }
    }

}
