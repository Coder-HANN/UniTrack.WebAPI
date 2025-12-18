using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetEventQrQueryHandler : IRequestHandler<GetEventQrQuery, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserService;
        private readonly IEventRepository eventRepository;

        public GetEventQrQueryHandler(
            ICurrentUserServices currentUserService,
            IEventRepository eventRepository)
        {
            this.currentUserService = currentUserService;
            this.eventRepository = eventRepository;
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
                    Message = "Yetkiniz yok."
                };
            }

            if (clubId == null)
            {
                return new ServiceResponse<string>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Kullanıcı bulunamadı."
                };
            }

            var eventEntity = eventRepository.GetEventByIdAndClubIdAsync(request.EventId, clubId.Value);

            if(eventEntity == null)
            {
                return new ServiceResponse<string>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Etkinlik bulunamadı."
                };
            }

            return new ServiceResponse<string>
            {
                Data = eventEntity.Result.QrCodeUrl,
                IsSuccess = true,
                Message = "Qr kod başarıyla getirildi."
            };
        }
    }
}
