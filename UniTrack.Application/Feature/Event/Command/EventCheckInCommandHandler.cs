using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Sheets;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Entities;

namespace UniTrack.Application.Feature.Event.Command.EventCheckInCommandHandler
{
    public class EventCheckInCommandHandler : IRequestHandler<EventCheckInCommand, ServiceResponse<string>>
    {
        private readonly IEventRepository eventRepository; // Etkinlik bilgisi için
        private readonly IUserRepository userRepository;   // Kullanıcı bilgisi için
        private readonly IParticipantSheetRepository sheetRepository; // Sheets güncellemesi için
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventUserRepository eventUserRepository;
        private readonly ILocalizationService localization; // Lokalizasyon servisi

        public EventCheckInCommandHandler(IEventRepository eventRepository,
            IUserRepository userRepository,
            IParticipantSheetRepository sheetRepository,
            ICurrentUserServices currentUserServices,
            IEventUserRepository eventUserRepository,
            ILocalizationService localizationService)
        {
            this.eventRepository = eventRepository;
            this.userRepository = userRepository;
            this.sheetRepository = sheetRepository;
            this.currentUserServices = currentUserServices;
            this.eventUserRepository = eventUserRepository;
            this.localization = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(EventCheckInCommand request, CancellationToken cancellationToken)
        {

            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return ServiceResponse<string>.Fail(await localization.Get(ValidationKeys.NotAuthorized));
            }

            // 1. Etkinliği Token ile bul
            var eventEntity = await eventRepository.GetCheckinIdAsync(request.EventCheckInId);
            if (eventEntity == null)
            {
                return ServiceResponse<string>.Fail(await localization.Get(ValidationKeys.EventNotFound));
            }

            // 2. Tarih kontrolü
            if (eventEntity.EndDate < DateTimeOffset.UtcNow)
            {
                return ServiceResponse<string>.Fail(await localization.Get(ValidationKeys.EventAlreadyEnded));
            }

            // 3. Kullanıcıyı getir
            var userEntity = await userRepository.GetByIdAsync(userId.Value);
            if (userEntity == null)
            {
                return ServiceResponse<string>.Fail(await localization.Get(ValidationKeys.UserNotFound));
            }

            // 4. Katılımcı kaydını (EventUser) bul
            // NOT: Repository'de Include(eu => eu.Event) olduğundan emin ol!
            var eventUser = await eventUserRepository.GetEventUserCheckInAsync(userId.Value, request.EventCheckInId);

            if (eventUser == null)
            {
                return ServiceResponse<string>.Fail(await localization.Get(ValidationKeys.UserNotJoinedEvent));
            }

            // 5. Mükerrer Check-in kontrolü
            if (eventUser.IsCheckedIn)
            {
                return ServiceResponse<string>.Fail(await localization.Get(ValidationKeys.AlreadyCheckedIn));
            }

            // 6. Güncelleme İşlemi
            try
            {
                eventUser.IsCheckedIn = true;
                eventUser.CheckedInAt = DateTimeOffset.UtcNow;

                await eventUserRepository.UpdateAsync(eventUser);

                // DİKKAT: Eğer UpdateAsync içinde SaveChanges yoksa buraya eklemelisin!
                // await unitOfWork.SaveChangesAsync(); 

            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.Fail("Veritabanı güncelleme hatası.");
            }

            // 7. Sheets Entegrasyonu (Arka plan işlemi gibi davranır)
            try
            {
                await sheetRepository.MarkUserAsCheckedInAsync(eventEntity.SheetsId, userEntity.Email);
            }
            catch (Exception ex)
            {
            }

            return ServiceResponse<string>.Success(await localization.Get(ValidationKeys.CheckInSuccess));
        }
    }
}
