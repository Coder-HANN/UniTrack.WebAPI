using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetAllEventCountQueryHandler : IRequestHandler<GetAllEventCountQuery, ServiceResponse<long>>
    {
        private readonly IEventRepository eventRepository;
        public GetAllEventCountQueryHandler(IEventRepository eventRepository)
        {
            this.eventRepository = eventRepository;
        }
        public async Task<ServiceResponse<long>> Handle(GetAllEventCountQuery request, CancellationToken cancellationToken)
        {
            var eventCount = await eventRepository.GetCountAsync();

            if (eventCount == 0)
            {
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
                Data = eventCount,
                Message = null
            };
        }
    }
}
