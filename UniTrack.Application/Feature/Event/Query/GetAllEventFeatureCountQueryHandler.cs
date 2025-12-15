using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetAllEventFeatureCountQueryHandler : IRequestHandler<GetAllEventCountQuery, ServiceResponse<long>>
    {
        private readonly IEventRepository eventRepository;
        public GetAllEventFeatureCountQueryHandler(IEventRepository eventRepository)
        {
            this.eventRepository = eventRepository;
        }
        public async Task<ServiceResponse<long>> Handle(GetAllEventCountQuery request, CancellationToken cancellationToken)
        {
            var count = await eventRepository.GetAllEventFeatureCountAsync();
            if (count == 0)
            {
                return new ServiceResponse<long>
                {
                    IsSuccess = false,
                    Data = 0,
                    Message = null
                };
            }

            return new ServiceResponse<long>
            {
                IsSuccess = true,
                Data = count,
                Message = null
            };
        }
    }
}
