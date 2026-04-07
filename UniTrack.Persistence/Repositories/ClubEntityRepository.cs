using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.DTOs.Club;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using UniTrack.Persistence.Context;

namespace UniTrack.Persistence.Repositories
{
    public class ClubEntityRepository : BaseEntityRepository<Club>, IClubRepository
    {
        public ClubEntityRepository(UniTrackDbContext context) : base(context)
        {
        }

        public Task<long> Get180DaysActiveClubsCountAsync()
        {
            var hundredEightyDaysAgo = DateTime.UtcNow.AddDays(-180);
            return dbSet.LongCountAsync(c => c.LastLoginDate != null && c.LastLoginDate >= hundredEightyDaysAgo);
        }

        public Task<long> Get30DaysActiveClubsCountAsync()
        {
            var thrityDaysAgo = DateTime.UtcNow.AddDays(-30);
            return dbSet.LongCountAsync(c => c.LastLoginDate != null && c.LastLoginDate >= thrityDaysAgo);
        }

        public Task<long> Get360DaysActiveClubsCountAsync()
        {
            var threeSixtyDaysAgo = DateTime.UtcNow.AddDays(-360);
            return dbSet.LongCountAsync(c => c.LastLoginDate != null && c.LastLoginDate >= threeSixtyDaysAgo);
        }

        public Task<long> Get90DaysActiveClubsCountAsync()
        {
            var ninetyDaysAgo = DateTime.UtcNow.AddDays(-90);
            return dbSet.LongCountAsync(c => c.LastLoginDate != null && c.LastLoginDate >= ninetyDaysAgo);
        }

        public Task<List<Club>> GetAllClubAsync(Expression<Func<Club, bool>> expression)
        {
            return dbSet.ToListAsync();
        }

        public async Task<List<Guid>> GetAllClubIdsAsync()
        {
            return await context.Clubs
                .AsNoTracking()
                .Select(c => c.Id)
                .ToListAsync();
        }

        public async Task<List<Club>> GetAllClubListAsync()
        {
            return await context.Clubs
                .Where(c => !c.IsDeleted && c.IsVerified)
                .Include(c => c.City)
                .Include(c => c.UserClubs)
                .Include(c => c.University)
                .ToListAsync();
        }

        public Task<Club> GetByEmailAndVerifyAsync(string ContectEmail)
        {
           return dbSet.FirstOrDefaultAsync(c => c.ContectEmail == ContectEmail  && c.IsVerified == true);
        }

        public Task<Club> GetByEmailAsync(string email)
        {
            return dbSet.FirstOrDefaultAsync(c => c.ContectEmail == email);
        }

        public Task<Club> GetByIdAsync(Guid Id)
        {
            return dbSet.FirstOrDefaultAsync(c => c.Id == Id);
        }

        public async Task<long> GetClubCountAsync()
        {
            return await context.Set<Club>()
                .Where(Club => !Club.IsDeleted && Club.IsVerified)
                .LongCountAsync();
        }

        public Task<int> GetClubFollowerCountAsync(Guid value)
        {
            return context.Set<UserClub>()
                .Where(cf => cf.ClubId == value)
                .CountAsync();
        }

        public async Task<List<Guid>> GetFilteredClubIdsAsync(List<int>? cityIds,List<Guid>? universityIds,List<Guid>? clubIds)
        {
            IQueryable<Club> query = context.Clubs
                .Where(c => !c.IsDeleted && c.IsVerified);

            if (cityIds != null && cityIds.Any())
            {
                query = query.Where(c => cityIds.Contains(c.CityId));
            }

            if (universityIds != null && universityIds.Any())
            {
                query = query.Where(c => universityIds.Contains(c.UniversityId));
            }

            if (clubIds != null && clubIds.Any())
            {
                query = query.Where(c => clubIds.Contains(c.Id));
            }

            return await query
                .Select(c => c.Id)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<Guid>> GetUserClubIdsAsync(Guid userId)
        {
            return await context.UserClubs
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.ClubId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<Club?> GetClubDetailByIdAsync(Guid clubId, Guid? userId)
        {
            return await dbSet
                .Include(c => c.University)
                .Include(c => c.City)
                .Include(c => c.Events)
                .Include(c => c.UserClubs)
                .FirstOrDefaultAsync(c => c.Id == clubId);
        }

        public async Task<List<MonthlyFollowerResponseDTO>> GetMonthlyFollowerCountAsync(Guid clubId)
        {
            var now = DateTimeOffset.UtcNow;
            // Başlangıç tarihini 11 ay öncesinin ilk gününe ayarla
            var startDate = new DateTimeOffset(now.AddMonths(-11).Year, now.AddMonths(-11).Month, 1, 0, 0, 0, TimeSpan.Zero);

            // 1. Veritabanından mevcut verileri çek
            var dbData = await context.UserClubs
                .Where(f => f.ClubId == clubId && f.CreatedDate >= startDate)
                .GroupBy(f => new { f.CreatedDate.Year, f.CreatedDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .ToListAsync();

            // 2. Son 12 ayın listesini manuel oluştur ve DB verisiyle eşleştir
            var result = new List<MonthlyFollowerResponseDTO>();

            for (int i = 0; i < 12; i++)
            {
                var targetDate = startDate.AddMonths(i);

                // Bu aya ait veri DB'den gelmiş mi kontrol et
                var match = dbData.FirstOrDefault(d => d.Year == targetDate.Year && d.Month == targetDate.Month);

                result.Add(new MonthlyFollowerResponseDTO
                {
                    Year = targetDate.Year,
                    Month = targetDate.Month.ToString(), // Gerekiyorsa targetDate.ToString("MMM") ile ay ismi dönebilirsin
                    Count = match?.Count ?? 0 // Veri yoksa 0 ata
                });
            }

            return result;
        }
        public async Task<List<GenderDistributionDTO>> GetFollowerGenderDistributionAsync(Guid clubId)
        {
            var followers = await context.UserClubs
                .Where(uc => uc.ClubId == clubId)
                .Join(context.UserDetails,
                    uc => uc.UserId,
                    ud => ud.UserId,
                    (uc, ud) => ud.Gender)
                .ToListAsync();

            var total = followers.Count;
            if (total == 0) return new List<GenderDistributionDTO>();

            return followers
                .GroupBy(g => g)
                .Select(g => new GenderDistributionDTO
                {
                    Label = g.Key.ToString(),
                    Count = g.Count(),
                    Percentage = Math.Round((double)g.Count() / total * 100, 1)
                })
                .OrderByDescending(x => x.Count)
                .ToList();
        }
    }

}
