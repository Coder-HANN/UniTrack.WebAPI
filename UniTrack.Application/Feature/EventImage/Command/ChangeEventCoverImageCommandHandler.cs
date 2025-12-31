using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.EventImage.Command
{
    public class ChangeEventCoverImageCommandHandler: IRequestHandler<ChangeEventCoverImageCommand, string>
    {
        private readonly IEventRepository eventRepository;
        private readonly IEventImageRepository eventImageRepository;
        private readonly ILocalizationService localization;
        private readonly ICurrentUserServices currentUserServices;

        public ChangeEventCoverImageCommandHandler(
            IEventRepository eventRepository,
            IEventImageRepository eventImageRepository,
            ILocalizationService localization,
            ICurrentUserServices currentUserServices)
        {
            this.eventRepository = eventRepository;
            this.eventImageRepository = eventImageRepository;
            this.localization = localization;
            this.currentUserServices = currentUserServices;
        }

        public async Task<string> Handle(ChangeEventCoverImageCommand request,CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();

            if (clubId == null)
                return await localization.Get(ValidationKeys.NotAuthorized);

            var eventEntity = await eventRepository.GetByIdAsync(request.EventId);

            if (eventEntity == null)
               return await localization.Get(ValidationKeys.EventNotFound);

            await eventImageRepository.SetCoverImageAsync(request.EventId,request.ImageId);

            return await localization.Get(ValidationKeys.EventUpdatedSuccess);
        }
    }
}