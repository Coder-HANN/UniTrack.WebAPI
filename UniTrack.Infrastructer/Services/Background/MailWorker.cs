using Microsoft.Extensions.Hosting;
using UniTrack.Application.Abstraction.Services.Mail;

namespace UniTrack.Infrastructure.Services.Background
{

    public class MailWorker : BackgroundService
    {
        private readonly IBackgroundMailQueue queue;
        private readonly IMailService mailService;

        public MailWorker(
            IBackgroundMailQueue queue,
            IMailService mailService)
        {
            this.queue = queue;
            this.mailService = mailService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var mail in queue.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await mailService.SendMailAsync(
                        mail.To,
                        mail.Subject,
                        mail.Body,
                        mail.IsHtml);
                }
                catch
                {
                    // LOG eklenebilir
                }
            }
        }
    }

}
