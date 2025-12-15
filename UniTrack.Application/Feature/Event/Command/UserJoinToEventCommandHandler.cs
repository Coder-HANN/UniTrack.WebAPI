using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Sheets;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Event.Command
{
    public class UserJoinToEventCommandHandler : IRequestHandler<UserJoinToEventCommand, ServiceResponse<UserJoinToEventResponseDTO>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventUserRepository eventUserRepository;
        private readonly IEventRepository eventRepository;
        private readonly IUserDetailRepository userDetailRepository;
        private readonly IParticipantSheetRepository participantSheetRepository;
        public UserJoinToEventCommandHandler(
            ICurrentUserServices currentUserServices,
            IEventUserRepository eventUserRepository,
            IEventRepository eventRepository,
            IUserDetailRepository userDetailRepository,
            IParticipantSheetRepository participantSheetRepository)
        {
            this.currentUserServices = currentUserServices;
            this.eventUserRepository = eventUserRepository;
            this.eventRepository = eventRepository;
            this.userDetailRepository = userDetailRepository;
            this.participantSheetRepository = participantSheetRepository;
        }
        public async Task<ServiceResponse<UserJoinToEventResponseDTO>> Handle(UserJoinToEventCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return new ServiceResponse<UserJoinToEventResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Unauthorized"
                };
            }
            var role = currentUserServices.Role();
            if (role == null || role == Role.Club)
            {
                return new ServiceResponse<UserJoinToEventResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Yetkisiz kullanıcı"
                };
            }

            var existingEntry = await eventUserRepository.GetAsync(eu => eu.EventId == request.EventId && eu.UserId == userId.Value);
            if (existingEntry != null)
            {
                return new ServiceResponse<UserJoinToEventResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Zaten bu etkinliğe katıldınız"
                };
            }
            var userUniversity = await userDetailRepository.GetAsync(ud=>ud.UserId == userId.Value);

            var eventEntity = await eventRepository.GetAsync(e => e.Id == request.EventId);

            if(eventEntity.EndDate < DateTime.UtcNow)
            {
                return new ServiceResponse<UserJoinToEventResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Etkinlik süresi doldu"
                };
            }

            var eventUser = await eventUserRepository.GetAsync(eu => eu.EventId == request.EventId);

            if (eventEntity.Quota <= eventEntity.Joiner) 
            {
                return new ServiceResponse<UserJoinToEventResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Katılımcı kotası doldu."
                };
            }

            if(eventEntity.Status != Status.Public && eventEntity.Club.UniversityId != userUniversity.UniverstiyId)
            {
                return new ServiceResponse<UserJoinToEventResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Üniversite öğrencilerine özel etkinlik"
                };
            }
            
            var response = new EventUser
            {
                EventId = request.EventId,
                UserId = userId.Value,
                IsJoined = true
            };

            await eventUserRepository.AddAsync(response);

            var add = await eventRepository.GetAsync(c => c.Id == request.EventId);

            add.Joiner = add.Joiner + 1;

            await eventRepository.UpdateAsync(add);

            // 3. GOOGLE SHEETS İŞLEMİ (Sheets Write)
            // Not: Etkinlik oluşturulurken SheetId'nin eventEntity.SheetId alanına yazıldığını varsayıyoruz.

            // a. Sheets'e yazılacak DTO'yu hazırlama
            if (!string.IsNullOrEmpty(eventEntity.SheetsId.ToString()))
            {
                // Bilgilerin null olmadığını kontrol ederek (defensive coding) DTO'yu hazırlıyoruz.
                var participantData = new SheetParticipantDTO
                {
                    Email = userUniversity.User?.Email, 
                    Name = $"{userUniversity.Name}",
                    Surname = userUniversity.Surname,
                    UniversityName = userUniversity.University?.Name,
                    DepartmentName = userUniversity.Department?.Name,
                    PhoneNumber = userUniversity.PhoneNumber, 
                    Graduaiton_Date = userUniversity.Graduaiton_Date,
                    JoinDate = DateTimeOffset.UtcNow // Düzeltilmiş yapı
                };

                try
                {
                    // Sheets'e yazma sorumluluğu IParticipantSheetRepository'ye devredildi.
                    await participantSheetRepository.AddParticipantAsync(eventEntity.SheetsId, participantData);
                }
                catch (Exception ex)
                {
                    // HATA YÖNETİMİ: Eğer Sheets'e yazarken kritik bir hata olursa,
                    // burada bir Transaction/Unit of Work kullanmıyorsak veritabanı kaydımız (DB) geçerli kalır.
                    // Bu senaryoda genellikle hata loglanır ve kullanıcıya başarılı yanıt dönülür (DB başarılı olduğu için), 
                    // ya da kritik hatalarda veritabanı işlemi geri alınır. Şimdilik loglama odaklı ilerleyelim.
                    // Logger.LogError(ex, "Etkinlik ID: {0} için Sheets'e yazılırken hata oluştu.", request.EventId);
                }
            }

            return new ServiceResponse<UserJoinToEventResponseDTO>
            {
                IsSuccess = true,
                Data = null,
                Message = "Kayıt başarıyla oluşturuldu"
            };
        }
    }
}
