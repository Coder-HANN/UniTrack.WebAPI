using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Storage;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.EventImage.Command
{
    public class DeleteEventImageCommandHandler: IRequestHandler<DeleteEventImageCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventImageRepository eventImageRepository;
        private readonly IEventRepository eventRepository;
        private readonly IStorageService storageService;
        private readonly ILocalizationService localization;

        public DeleteEventImageCommandHandler(
            ICurrentUserServices currentUserServices,
            IEventImageRepository eventImageRepository,
            IEventRepository eventRepository,
            IStorageService storageService,
            ILocalizationService localization)
        {
            this.currentUserServices = currentUserServices;
            this.eventImageRepository = eventImageRepository;
            this.eventRepository = eventRepository;
            this.storageService = storageService;
            this.localization = localization;
        }

        public async Task<ServiceResponse<string>> Handle(DeleteEventImageCommand request,CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();
            var role = currentUserServices.Role();

            if (clubId == null || role == null || role == Role.User)
            {
                return ServiceResponse<string>.Fail(
                    await localization.Get(ValidationKeys.NotAuthorized));
            }

            var image = await eventImageRepository.GetByIdAsync(request.ImageId);

            if (image == null || image.EventId != request.EventId)
            {
                return ServiceResponse<string>.Fail(await localization.Get(ValidationKeys.ImageNotFound));
            }

            var eventEntity = await eventRepository.GetByIdAsync(request.EventId);

            if (eventEntity == null || eventEntity.ClubId != clubId)
            {
                return ServiceResponse<string>.Fail(await localization.Get(ValidationKeys.NotAuthorized));
            }

            bool wasCover = image.IsCover;

            // 1️⃣ DB’den sil
            await eventImageRepository.DeleteAsync(image);

            // 2️⃣ Storage’dan sil
            if (!string.IsNullOrEmpty(image.ImageUrl))
            {
                await storageService.DeleteFileAsync(image.ImageUrl);
            }

            // 3️⃣ Cover silindiyse → yeni cover belirle
            if (wasCover)
            {
                var remainingImages =
                    await eventImageRepository.GetByEventIdAsync(request.EventId);

                var newCover = remainingImages
                    .OrderBy(x => x.Order)
                    .FirstOrDefault();

                if (newCover != null)
                {
                    newCover.IsCover = true;
                    await eventImageRepository.UpdateRangeAsync(new List<Domain.Entities.EventImage> { newCover });
                }
            }

            return ServiceResponse<string>.Success(await localization.Get(ValidationKeys.OperationSuccessful));
        }
    }

}
