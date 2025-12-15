using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetAllEventQueryHandler : IRequestHandler<GetAllEventQuery, ServiceResponse<IPagingExecutionResult<GetAllEventQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventRepository eventRepository;
        private readonly BaseEntityRepository<Domain.Entities.Event> baseEntityRepository;

        public GetAllEventQueryHandler(
            ICurrentUserServices currentUserServices,
            IEventRepository eventRepository,
            BaseEntityRepository<Domain.Entities.Event> baseEntityRepository)
        {
            this.currentUserServices = currentUserServices;
            this.eventRepository = eventRepository;
            this.baseEntityRepository = baseEntityRepository;
        }
        public async Task<ServiceResponse<IPagingExecutionResult<GetAllEventQueryResponseDTO>>> Handle(GetAllEventQuery request, CancellationToken cancellationToken)
        {
            
            var events = await eventRepository.GetAllEventAsync(e => e.IsDeleted== false);
            if (events == null || events.Count == 0)
            {
                return new ServiceResponse<IPagingExecutionResult<GetAllEventQueryResponseDTO>> {

                        IsSuccess = false,
                        Data = null,
                        Message = "No events found"
                };
            }


            var responses = events.Select(e => new GetAllEventQueryResponseDTO
            {

                Image = e.Image,
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Location = e.Location,
                Quota = e.Quota,
                ClubId = e.ClubId,  // TO DO: Düzenlenecek
                Tags = e.Tag,
                Time = e.Time,
                Status = e.Status,

            });

            var result = await baseEntityRepository.GetPagedResult(
              responses,
              pageSize: request.PageSize,
              pageIndex: request.Page,
              ordering: q => q.OrderByDescending(x => x.StartDate),
              cancellationToken: cancellationToken
            );

            return new ServiceResponse<IPagingExecutionResult<GetAllEventQueryResponseDTO>>
            {
                IsSuccess = true,
                Data = result,
                Message = null
            };
        }
    }
}
