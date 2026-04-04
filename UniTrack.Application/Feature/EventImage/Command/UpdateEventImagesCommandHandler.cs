using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.EventImage.Command
{
    public class UpdateEventImagesCommandHandler : IRequestHandler<UpdateEventImagesCommand, ServiceResponse<string>>
    {
        private readonly IEventImageRepository _eventImageRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ICurrentUserServices _currentUserServices;
        private readonly ILocalizationService _localizationService;

        public UpdateEventImagesCommandHandler(
            IEventImageRepository eventImageRepository,
            IEventRepository eventRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService)
        {
            _eventImageRepository = eventImageRepository;
            _eventRepository = eventRepository;
            _currentUserServices = currentUserServices;
            _localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(
            UpdateEventImagesCommand request, CancellationToken cancellationToken)
        {
            var clubId = _currentUserServices.CurrentClub();
            if (clubId == null)
                return ServiceResponse<string>.Fail(
                    await _localizationService.Get(ValidationKeys.NotAuthorized));

            var evnt = await _eventRepository.GetByIdAsync(request.EventId);
            if (evnt == null)
                return ServiceResponse<string>.Fail(
                    await _localizationService.Get(ValidationKeys.EventNotFound));

            await _eventImageRepository.DeleteByEventIdAsync(evnt.Id);

            var images = request.ImageUrls.Select((url, index) => new Domain.Entities.EventImage
            {
                EventId = evnt.Id,
                ImageUrl = url,
                IsCover = index == 0,
                Order = index + 1
            }).ToList();

            await _eventImageRepository.AddRangeAsync(images);

            return ServiceResponse<string>.Success(
                await _localizationService.Get(ValidationKeys.OperationSuccessful));
        }
    }
}