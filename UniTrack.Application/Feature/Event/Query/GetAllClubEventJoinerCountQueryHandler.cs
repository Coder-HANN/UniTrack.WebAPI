using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetAllClubEventJoinerCountQueryHandler : IRequestHandler<GetAllClubEventJoinerCountQuery, ServiceResponse<long>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventRepository eventRepository;
        private readonly ILocalizationService localizationService;
        public GetAllClubEventJoinerCountQueryHandler(ICurrentUserServices currentUserServices, IEventRepository eventRepository, ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.eventRepository = eventRepository;
            this.localizationService = localizationService;
        }

        // Kulüp etkinliklerine katılan toplam kişi sayısını döner
        public async Task<ServiceResponse<long>> Handle(GetAllClubEventJoinerCountQuery request, CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();
            var role = currentUserServices.Role();

            if (clubId == null && role != Role.Club && role != Role.Admin) { 
            
                return new ServiceResponse<long>
                {
                    IsSuccess = false,
                    Data = 0,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            var checkedInCount = await eventRepository.GetClubEventCheckedInCountAsync(clubId);

            if (checkedInCount == 0) {
                return new ServiceResponse<long>
                {
                    IsSuccess = true,
                    Data = 0,
                    Message = null
                };
            }

            return new ServiceResponse<long>
            {
                IsSuccess = true,
                Data = checkedInCount,
                Message = null
            };
        }
    }
}
