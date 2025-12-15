using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetAllPastEventQueryHandler : IRequestHandler<GetAllPastEventQuery, ServiceResponse<IPagingExecutionResult<List<GetAllPastEventQueryResponseDTO>>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly BaseEntityRepository<Domain.Entities.Event> pagingBaseService;
        private readonly IEventRepository eventRepository;
        public GetAllPastEventQueryHandler(
            ICurrentUserServices currentUserServices,
            BaseEntityRepository<Domain.Entities.Event> pagingBaseService,
            IEventRepository eventRepository)
        {
            this.currentUserServices = currentUserServices;
            this.pagingBaseService = pagingBaseService;
            this.eventRepository = eventRepository;
        }
        public async Task<ServiceResponse<IPagingExecutionResult<List<GetAllPastEventQueryResponseDTO>>>> Handle(GetAllPastEventQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return new ServiceResponse<IPagingExecutionResult<List<GetAllPastEventQueryResponseDTO>>>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Unauthorized"
                };
            }
            var events = await eventRepository.GetPastEventsAsync();
            if (events == null || events.Count == 0)
            {
                return new ServiceResponse<IPagingExecutionResult<List<GetAllPastEventQueryResponseDTO>>>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "No featured events found"
                };
            }
            var responses = events.Select(e => new GetAllPastEventQueryResponseDTO
            {
                Image = e.Image,
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Location = e.Location,
                Quota = e.Quota,
                ClubId = e.ClubId,
                Tags = e.Tag,
                Time = e.Time,
                Status = e.Status,
            }).ToList();

            var result = await pagingBaseService.GetPagedResult(
              responses,
              pageSize: request.PageSize,
              pageIndex: request.Page,
              ordering: q => q.OrderByDescending(x => x.StartDate),
              cancellationToken: cancellationToken
            );

            return new ServiceResponse<IPagingExecutionResult<List<GetAllPastEventQueryResponseDTO>>>
            {
                IsSuccess = true,
                Data = (IPagingExecutionResult<List<GetAllPastEventQueryResponseDTO>>)result,
                Message = "Past events retrieved successfully"
            };
        }
    }
}
