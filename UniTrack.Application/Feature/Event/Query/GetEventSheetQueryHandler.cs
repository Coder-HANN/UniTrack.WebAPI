using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using System.Threading;
using System.Threading.Tasks;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetEventSheetQueryHandler : IRequestHandler<GetEventSheetQuery, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserService;
        private readonly IEventRepository eventRepository;

        // Google Sheets Görüntüleme URL'sinin sabit kısmı.
        private const string BaseGoogleSheetUrl = "https://docs.google.com/spreadsheets/d/";

        public GetEventSheetQueryHandler(ICurrentUserServices currentUserService, IEventRepository eventRepository)
        {
            this.currentUserService = currentUserService;
            this.eventRepository = eventRepository;
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
                    Message = "Yetkiniz yok."
                };
            }
            if (clubId == null)
            {
                return new ServiceResponse<string>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Kullanıcı/Kulüp bilgisi bulunamadı."
                };
            }

            var eventEntity = await eventRepository.GetEventByIdAndClubIdAsync(request.EventId, clubId.Value);

            if (eventEntity == null)
            {
                return new ServiceResponse<string>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Etkinlik bulunamadı veya yetkiniz olmayan bir etkinliği istiyorsunuz."
                };
            }

            // 3. Sheets ID Kontrolü
            if (string.IsNullOrEmpty(eventEntity.SheetsId))
            {
                return new ServiceResponse<string>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Bu etkinlik için Google Sheets e-tablo ID'si bulunamadı."
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
                Message = "E-tablo URL'si başarıyla getirildi."
            };
        }
    }
}