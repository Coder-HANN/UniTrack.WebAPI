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
    public class DeleteEventImageCommandHandler : IRequestHandler<DeleteEventImageCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices _currentUserServices;
        private readonly IEventImageRepository _eventImageRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IStorageService _storageService;
        private readonly ILocalizationService _localizationService;

        public DeleteEventImageCommandHandler(
            ICurrentUserServices currentUserServices,
            IEventImageRepository eventImageRepository,
            IEventRepository eventRepository,
            IStorageService storageService,
            ILocalizationService localizationService)
        {
            _currentUserServices = currentUserServices;
            _eventImageRepository = eventImageRepository;
            _eventRepository = eventRepository;
            _storageService = storageService;
            _localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(
            DeleteEventImageCommand request, CancellationToken cancellationToken)
        {
            var clubId = _currentUserServices.CurrentClub();
            var role = _currentUserServices.Role();
            if (clubId == null || role == null || role == Role.User)
                return ServiceResponse<string>.Fail(
                    await _localizationService.Get(ValidationKeys.NotAuthorized));

            var image = await _eventImageRepository.GetByIdAsync(request.ImageId);
            if (image == null || image.EventId != request.EventId)
                return ServiceResponse<string>.Fail(
                    await _localizationService.Get(ValidationKeys.ImageNotFound));

            var eventEntity = await _eventRepository.GetByIdAsync(request.EventId);
            if (eventEntity == null || eventEntity.ClubId != clubId)
                return ServiceResponse<string>.Fail(
                    await _localizationService.Get(ValidationKeys.NotAuthorized));

            bool wasCover = image.IsCover;

            await _eventImageRepository.DeleteAsync(image);

            if (!string.IsNullOrEmpty(image.ImageUrl))
                await _storageService.DeleteFileAsync(image.ImageUrl);

            if (wasCover)
            {
                var remainingImages = await _eventImageRepository.GetByEventIdAsync(request.EventId);
                var newCover = remainingImages.OrderBy(x => x.Order).FirstOrDefault();
                if (newCover != null)
                {
                    newCover.IsCover = true;
                    await _eventImageRepository.UpdateRangeAsync(new List<Domain.Entities.EventImage> { newCover });
                }
            }

            return ServiceResponse<string>.Success(
                await _localizationService.Get(ValidationKeys.OperationSuccessful));
        }
    }
}