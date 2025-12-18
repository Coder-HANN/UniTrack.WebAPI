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

            var eventEntity = await eventRepository.GetByIdAsync(request.EventCheckInId);

            var userEntity = await userRepository.GetByIdAsync(userId.Value);

            if (eventEntity == null)
            {
                return ServiceResponse<string>.Fail(await localization.Get(ValidationKeys.EventNotFound));
            }
            var eventUser = await eventUserRepository.GetEventUserCheckInAsync(userId.Value, request.EventCheckInId);
            if (eventUser != null)
            {
                return ServiceResponse<string>.Fail(ValidationKeys.AlreadyCheckedIn);
            }

            await sheetRepository.MarkUserAsCheckedInAsync(eventEntity.SheetsId,userEntity.Email); // Sheets'te kayıtlı olan eşsiz değer (genellikle Email)

            await eventUserRepository.UpdateAsync(new EventUser
            {
                EventId = request.EventCheckInId,
                UserId = userId.Value,
                IsJoined = true,
                IsCheckedIn = true,
                CheckedInAt = DateTime.UtcNow
            });

            return ServiceResponse<string>.Success(await localization.Get(ValidationKeys.CheckInSuccess));
        }
    }
}
