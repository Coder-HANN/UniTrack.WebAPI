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

        public async Task<List<Notification>> GetMatchingNotificationsAsync(int? cityId, Guid? universityId, int? departmentId, List<Guid> clubIds)
        {
           
            var query = context.TargetNotifications
                .Include(t => t.Notification)
                .Include(t => t.City)
                .Include(t => t.Universities)
                .Include(t => t.Departments)
                .Include(t => t.Clubs)
                .AsQueryable();

            query = query.Where(t =>
                // 🔹 Hiç filtre yoksa → herkese
                (!t.City.Any()
                 && !t.Universities.Any()
                 && !t.Departments.Any()
                 && !t.Clubs.Any())

                // 🔹 City match
                || t.City.Any(c => c.CityId == cityId)

                // 🔹 University match
                || t.Universities.Any(u => u.UniversityId == universityId)

                // 🔹 Department match
                || t.Departments.Any(d => d.DepartmentId == departmentId)

                // 🔹 Club match (kullanıcı birden fazla kulüpte olabilir)
                || t.Clubs.Any(c => clubIds.Contains(c.ClubId))
            );

            return await query
                .Select(t => t.Notification)
                .Distinct()
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

    }
}

