using Microsoft.Extensions.DependencyInjection; // Gerekli namespace
using Microsoft.Extensions.Hosting;
using UniTrack.Application.Abstraction.Services.Mail;

namespace UniTrack.Infrastructure.Services.Background
{
    public class MailWorker : BackgroundService
    {
        private readonly IBackgroundMailQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory; // Factory eklendi

        public MailWorker(
            IBackgroundMailQueue queue,
            IServiceScopeFactory scopeFactory) // MailService yerine scopeFactory istiyoruz
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var mail in _queue.Reader.ReadAllAsync(stoppingToken))
            {
                // Her bir mail için yeni bir scope oluşturuyoruz
                using (var scope = _scopeFactory.CreateScope())
                {
                    try
                    {
                        // IMailService'i bu scope içerisinden talep ediyoruz
                        var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();

                        await mailService.SendMailAsync(
                            mail.To,
                            mail.Subject,
                            mail.Body,
                            mail.IsHtml);
                    }
                    catch (Exception ex)
                    {
                        // Buraya logger eklemeni öneririm
                        // Örneğin: Console.WriteLine($"Mail gönderimi başarısız: {ex.Message}");
                    }
                } // using bloğu sonunda mailService ve bağımlılıkları (DbContext vb.) güvenle temizlenir
            }
        }
    }
}