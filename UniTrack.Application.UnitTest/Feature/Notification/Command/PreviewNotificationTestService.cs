using UniTrack.Application.Abstraction.Services.Notification;
using UniTrack.Application.DTOs.Notification;

namespace UniTrack.Infrastructure.Services.Notification
{
    public class PreviewNotificationTestService : IPreviewNotificationService
    {
        public Task<PreviewTargetNotificationResponseDTO> PreviewAsync(
            List<int>? cityIds,
            List<Guid>? universityIds,
            List<int>? departmentIds,
            List<Guid>? clubIds)
        {
            // Fake logic (test için)
            var clubCount = clubIds?.Count ?? 10;
            var userCount = clubCount * 25;

            return Task.FromResult(new PreviewTargetNotificationResponseDTO
            {
                ClubCount = clubCount,
                UserCount = userCount
            });
        }
    }
}
