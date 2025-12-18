using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Command
{
    public class EventIsClubCountCommandHandler : IRequestHandler<EventIsClubCountCommand, ServiceResponse<EventIsClubCountResponseDTO>>
    {
        private readonly IEventRepository eventRepository;
        public EventIsClubCountCommandHandler(IEventRepository eventRepository)
        {
            this.eventRepository = eventRepository;
        }

        // Kulübün tüm etkinlik sayısını döner

        public async Task<ServiceResponse<EventIsClubCountResponseDTO>> Handle(EventIsClubCountCommand request, CancellationToken cancellationToken)
        {
            var count = await eventRepository.GetClubEventCountAsync(request.ClubId);

            var responseDto = new EventIsClubCountResponseDTO
            {
                EventCount = count
            };

            return new ServiceResponse<EventIsClubCountResponseDTO>
            {
                IsSuccess = true,
                Data = responseDto,
                Message = null
            };
        }
    }
}
