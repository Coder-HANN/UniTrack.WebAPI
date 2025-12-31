using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.EventImage.Command
{
    public class UpdateEventImagesCommandHandler: IRequestHandler<UpdateEventImagesCommand, ServiceResponse<string>>
    {
        private readonly IEventImageRepository eventImageRepository;
        private readonly IEventRepository eventRepository;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localization;

        public UpdateEventImagesCommandHandler(
            IEventImageRepository eventImageRepository,
            IEventRepository eventRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localization)
        {
            this.eventImageRepository = eventImageRepository;
            this.eventRepository = eventRepository;
            this.currentUserServices = currentUserServices;
            this.localization = localization;
        }

        public async Task<ServiceResponse<string>> Handle(UpdateEventImagesCommand request,CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();
            if (clubId == null)
                return ServiceResponse<string>.Fail(await localization.Get(ValidationKeys.NotAuthorized));

            var evnt = await eventRepository.GetByIdAsync(request.EventId);

            if (evnt == null)
                return ServiceResponse<string>.Fail(await localization.Get(ValidationKeys.EventNotFound));

            // 1️⃣ Eski image’ları sil
            await eventImageRepository.DeleteByEventIdAsync(evnt.Id);

            // 2️⃣ Yenileri ekle
            var images = request.ImageUrls.Select((url, index) => new Domain.Entities.EventImage
            {
                EventId = evnt.Id,
                ImageUrl = url,
                IsCover = index == 0,
                Order = index + 1
            }).ToList();

            await eventImageRepository.AddRangeAsync(images);

            return ServiceResponse<string>.Success(await localization.Get(ValidationKeys.OperationSuccessful));
        }
    }

}
