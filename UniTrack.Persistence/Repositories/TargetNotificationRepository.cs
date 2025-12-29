using Microsoft.EntityFrameworkCore;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class TargetNotificationRepository : BaseEntityRepository<TargetNotification>, ITargetNotificationRepository
    {
        public TargetNotificationRepository(UniTrackDbContext context) : base(context)
        {
        }

        public async Task<List<TargetNotification>> GetMatchingNotificationsAsync(int? cityId, Guid? universityId, int? departmentId, List<Guid> clubIds)
        {
            return await context.TargetNotifications
                .Where(n =>
                    (!n.City.Any() || (cityId != null && n.City.Any(c => c.CityId == cityId)))
                    && (!n.Universities.Any() || (universityId != null && n.Universities.Any(u => u.UniversityId == universityId)))
                    && (!n.Departments.Any() || (departmentId != null && n.Departments.Any(d => d.DepartmentId == departmentId)))
                    && (!n.Clubs.Any() || (clubIds.Any() && n.Clubs.Any(c => clubIds.Contains(c.ClubId))))
                )
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<TargetNotification>> GetMatchingNotificationsForClubAsync(int? cityId, Guid? universityId)
        {
            return await context.TargetNotifications
               .Where(n =>
                   (!n.City.Any() || (cityId != null && n.City.Any(c => c.CityId == cityId)))
                   && (!n.Universities.Any() || (universityId != null && n.Universities.Any(u => u.UniversityId == universityId)))
               )
               .OrderByDescending(n => n.CreatedDate)
               .ToListAsync();
        }
    }
}