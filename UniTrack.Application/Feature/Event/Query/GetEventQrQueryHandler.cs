using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetEventQrQueryHandler : IRequestHandler<GetEventQrQuery, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserService;
        private readonly IEventRepository eventRepository;
        private readonly ILocalizationService localizationService;

        public GetEventQrQueryHandler(
            ICurrentUserServices currentUserService,
            IEventRepository eventRepository,
            ILocalizationService localizationService)
        {
            this.currentUserService = currentUserService;
            this.eventRepository = eventRepository;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<string>> Handle(GetEventQrQuery request, CancellationToken cancellationToken)
        {
            var clubId = currentUserService.CurrentClub();
            var role = currentUserService.Role();

            if(role != Domain.Enums.Role.Club)
            {
                return new ServiceResponse<string>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            if (clubId == null)
            {
                return new ServiceResponse<string>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.UserNotFound)
                };
            }

            var eventEntity = await eventRepository.GetEventByIdAndClubIdAsync(request.EventId, clubId.Value);

            if(eventEntity == null)
            {
                return new ServiceResponse<string>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.EventNotFound)
                };
            }

            return new ServiceResponse<string>
            {
                Data = eventEntity.QrCodeUrl,
                IsSuccess = true,
                Message = null
            };
        }
    }
}
