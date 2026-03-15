using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UniTrack.Domain.Enums;
using UniTrack.Persistence.Context;

namespace UniTrack.Infrastructure.Services.Background
{
    public class EventTimeUpdateBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EventTimeUpdateBackgroundService> _logger;

        public EventTimeUpdateBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<EventTimeUpdateBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<UniTrackDbContext>(); // kendi DbContext adınız

                    var now = DateTime.UtcNow;

                    var eventsToUpdate = await dbContext.Events
                    .Where(e => e.Time != Time.Cancelled
                        && (
                            (e.StartDate <= now && e.EndDate >= now && e.Time != Time.Ongoing) ||
                            (e.EndDate < now && e.Time != Time.Past)
                        ))
                    .ToListAsync(stoppingToken);

                    foreach (var e in eventsToUpdate)
                    {
                        if (e.EndDate < now)
                            e.Time = Time.Past;
                        else if (e.StartDate <= now && e.EndDate >= now)
                            e.Time = Time.Ongoing;
                    }

                    foreach (var e in eventsToUpdate)
                    {
                        if (e.EndDate < now)
                            e.Time = Time.Past;
                        else if (e.StartDate <= now && e.EndDate >= now)
                            e.Time = Time.Ongoing;
                    }

                    if (eventsToUpdate.Any())
                    {
                        await dbContext.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("{Count} etkinlik güncellendi.", eventsToUpdate.Count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "EventTimeUpdateBackgroundService hata.");
                }

                // Her 5 dakikada bir çalış
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}