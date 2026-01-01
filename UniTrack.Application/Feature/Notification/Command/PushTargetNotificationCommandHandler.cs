using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Repositories;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class PushTargetNotificationCommandHandler: IRequestHandler<PushTargetNotificationCommand, ServiceResponse<bool>>
    {
        private readonly INotificationRepository notificationRepository;
        private readonly ITargetNotificationRepository targetNotificationRepository;
        private readonly ITargetNotificationCityRepository targetNotificationCityRepository;
        private readonly ITargetNotificationUniversityRepository targetNotificationUniversityRepository;
        private readonly ITargetNotificationDepartmentRepository targetNotificationDepartmentRepository;
        private readonly ITargetNotificationClubRepository targetNotificationClubRepository;
        private readonly ICurrentUserServices currentUserServices;

        public PushTargetNotificationCommandHandler(
            INotificationRepository notificationRepository,
            ITargetNotificationRepository targetNotificationRepository,
            ITargetNotificationCityRepository targetNotificationCityRepository,
            ITargetNotificationUniversityRepository targetNotificationUniversityRepository,
            ITargetNotificationDepartmentRepository targetNotificationDepartmentRepository,
            ITargetNotificationClubRepository targetNotificationClubRepository,
            ICurrentUserServices currentUserServices)
        {
            this.notificationRepository = notificationRepository;
            this.targetNotificationRepository = targetNotificationRepository;
            this.targetNotificationCityRepository = targetNotificationCityRepository;
            this.targetNotificationUniversityRepository = targetNotificationUniversityRepository;
            this.targetNotificationDepartmentRepository = targetNotificationDepartmentRepository;
            this.targetNotificationClubRepository = targetNotificationClubRepository;
            this.currentUserServices = currentUserServices;
        }
        // Kullanıcıya toplu bildirim gönderir

        public async Task<ServiceResponse<bool>> Handle(PushTargetNotificationCommand request,CancellationToken cancellationToken)
        {
            var adminId = currentUserServices.CurrentUser();
            if (adminId == null)
            {
                return ServiceResponse<bool>.Fail(ValidationKeys.NotAuthorized);
            }
            var role = currentUserServices.Role();
            if (role != Domain.Enums.Role.Admin)
            {
                return ServiceResponse<bool>.Fail(ValidationKeys.NotAuthorized);
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

            // 5️⃣ Department
            if (request.DepartmentId.HasValue)
            {
                await targetNotificationDepartmentRepository.AddAsync(
                    new TargetNotificationDepartment
                    {
                        Id = Guid.NewGuid(),
                        TargetNotificationId = target.Id,
                        DepartmentId = request.DepartmentId.Value
                    });
            }

            // 6️⃣ Clubs
            if (request.ClubIds.Any())
            {
                var clubs = request.ClubIds.Select(clubId =>
                    new TargetNotificationClub
                    {
                        Id = Guid.NewGuid(),
                        TargetNotificationId = target.Id,
                        ClubId = clubId
                    }).ToList();

                await targetNotificationClubRepository.AddRangeAsync(clubs);
            }

            

            return ServiceResponse<bool>.Success(null,true);
        }
    }
}
