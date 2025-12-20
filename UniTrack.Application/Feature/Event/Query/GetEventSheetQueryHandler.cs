using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetEventSheetQueryHandler : IRequestHandler<GetEventSheetQuery, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserService;
        private readonly IEventRepository eventRepository;
        private readonly ILocalizationService localizationService;

        // Google Sheets Görüntüleme URL'sinin sabit kısmı.
        private const string BaseGoogleSheetUrl = "https://docs.google.com/spreadsheets/d/";

        public GetEventSheetQueryHandler(
            ICurrentUserServices currentUserService, 
            IEventRepository eventRepository,
            ILocalizationService localizationService)
        {
            this.currentUserService = currentUserService;
            this.eventRepository = eventRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(GetEventSheetQuery request, CancellationToken cancellationToken)
        {
            // 1. Yetkilendirme ve Kullanıcı Kontrolü
            var clubId = currentUserService.CurrentClub();
            var role = currentUserService.Role();

            if (role != Domain.Enums.Role.Club)
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
                    Message = await localizationService.Get(ValidationKeys.ClubNotFound)
                };
            }

            var eventEntity = await eventRepository.GetEventByIdAndClubIdAsync(request.EventId, clubId.Value);

            if (eventEntity == null)
            {
                return new ServiceResponse<string>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.EventNotFound)
                };
            }

            // 3. Sheets ID Kontrolü
            if (string.IsNullOrEmpty(eventEntity.SheetsId))
            {
                return new ServiceResponse<string>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.GoogleSheetsTableNotFound)
                };
            }

            // 4. Sheets URL'sini Oluştur
            // Örn: https://docs.google.com/spreadsheets/d/{SheetsId}/edit
            string fullSheetUrl = $"{BaseGoogleSheetUrl}{eventEntity.SheetsId}/edit";

            // 5. Başarılı Yanıtı Dön
            return new ServiceResponse<string>
            {
                Data = fullSheetUrl,
                IsSuccess = true,
                Message = null
            };
        }
    }
}