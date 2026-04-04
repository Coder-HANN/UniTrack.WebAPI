using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.EventImage.Command
{
    public class ChangeEventCoverImageCommandHandler : IRequestHandler<ChangeEventCoverImageCommand, ServiceResponse<string>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventImageRepository _eventImageRepository;
        private readonly ILocalizationService _localizationService;
        private readonly ICurrentUserServices _currentUserServices;

        public ChangeEventCoverImageCommandHandler(
            IEventRepository eventRepository,
            IEventImageRepository eventImageRepository,
            ILocalizationService localizationService,
            ICurrentUserServices currentUserServices)
        {
            _eventRepository = eventRepository;
            _eventImageRepository = eventImageRepository;
            _localizationService = localizationService;
            _currentUserServices = currentUserServices;
        }

        public async Task<ServiceResponse<string>> Handle(
            ChangeEventCoverImageCommand request, CancellationToken cancellationToken)
        {
            var clubId = _currentUserServices.CurrentClub();
            if (clubId == null)
                return ServiceResponse<string>.Fail(
                    await _localizationService.Get(ValidationKeys.NotAuthorized));

            var eventEntity = await _eventRepository.GetByIdAsync(request.EventId);
            if (eventEntity == null)
                return ServiceResponse<string>.Fail(
                    await _localizationService.Get(ValidationKeys.EventNotFound));

            await _eventImageRepository.SetCoverImageAsync(request.EventId, request.ImageId);

            return ServiceResponse<string>.Success(
                await _localizationService.Get(ValidationKeys.EventUpdatedSuccess));
        }
    }
}