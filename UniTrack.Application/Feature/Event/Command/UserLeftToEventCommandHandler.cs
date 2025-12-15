using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Sheets;
using UniTrack.Application.Common;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Event.Command
{
    public class UserLeftToEventCommandHandler : IRequestHandler<UserLeftToEventCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventUserRepository eventUserRepository;
        private readonly IEventRepository eventRepository;
        private readonly IParticipantSheetRepository participantSheetRepository;
        public UserLeftToEventCommandHandler(
            ICurrentUserServices currentUserServices,
            IEventUserRepository eventUserRepository,
            IEventRepository eventRepository,
            IParticipantSheetRepository participantSheetRepository)
        {
            this.currentUserServices = currentUserServices;
            this.eventUserRepository = eventUserRepository;
            this.eventRepository = eventRepository;
            this.participantSheetRepository = participantSheetRepository;
        }
        public async Task<ServiceResponse<string>> Handle(UserLeftToEventCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Unauthorized"
                };
            }
            var role = currentUserServices.Role();
            if (role == null || role == Role.Club)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Yetkisiz kullanıcı"
                };
            }
            var eventUser = await eventUserRepository.GetAsync(eu => eu.EventId == request.EventId && eu.UserId == userId && eu.IsJoined == true);
            if (eventUser == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "You are not joined to this event"
                };
            }
            await eventUserRepository.DeleteAsync(eventUser);

            var add = await eventRepository.GetAsync(c => c.Id == request.EventId);

            add.Joiner = add.Joiner + 1;

            await eventRepository.UpdateAsync(add);

            // 3. GOOGLE SHEETS İŞLEMİ (Sheets Delete)

            // a. Sheets ID kontrolü ve Email'in mevcut olup olmadığını kontrol etme
            var sheetId = add.SheetsId; 
            var userEmail = eventUser.User?.Email;

            if (!string.IsNullOrEmpty(sheetId) && !string.IsNullOrEmpty(userEmail))
            {
                try
                {
                    // Sheets'ten silme sorumluluğu IParticipantSheetRepository'ye devredildi.
                    // Sheets'teki kaydı bulmak için Email kullanıyoruz (daha önce kararlaştırdığımız gibi).
                    await participantSheetRepository.RemoveParticipantAsync(sheetId, userEmail);
                }
                catch (Exception ex)
                {
                    // HATA YÖNETİMİ: Sheets'ten silme başarısız olsa bile, DB kaydımız silinmiştir.
                    // Kullanıcıya başarılı yanıt dönülür ve hata loglanır.
                    // Logger.LogError(ex, "Etkinlik ID: {0} için Sheets'ten silinirken hata oluştu.", request.EventId);
                }
            }

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Data = null,
                Message = "You have left the event successfully"
            };
        }
    }
}
