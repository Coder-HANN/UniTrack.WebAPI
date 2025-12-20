using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetAllEventPastCountQueryHandler : IRequestHandler<GetAllEventPastCountQuery, ServiceResponse<long>>
    {
        private readonly IEventRepository eventRepository;
        public GetAllEventPastCountQueryHandler(IEventRepository eventRepository)
        {
            this.eventRepository = eventRepository;
        }
        public async Task<ServiceResponse<long>> Handle(GetAllEventPastCountQuery request, CancellationToken cancellationToken)
        {
            var pastEventsCount = await eventRepository.GetAllPastEventCountAsync();

            if (pastEventsCount == 0||pastEventsCount== null)
            {
                return new ServiceResponse<long>
                {
                    IsSuccess = true,
                    Data = 0,
                    Message =null
                };
            }

            return new ServiceResponse<long>
            {
                IsSuccess = true,
                Data = pastEventsCount,
                Message = null
            };
        }
    }
}
