using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetAllEventQueryHandler : IRequestHandler<GetAllEventQuery, ServiceResponse<IPagingExecutionResult<GetAllEventQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventRepository eventRepository;
        private readonly IBaseEntityRepository<Domain.Entities.Event> baseEntityRepository;
        private readonly ILocalizationService localizationService;

        public GetAllEventQueryHandler(
            ICurrentUserServices currentUserServices,
            IEventRepository eventRepository,
            IBaseEntityRepository<Domain.Entities.Event> baseEntityRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.eventRepository = eventRepository;
            this.baseEntityRepository = baseEntityRepository;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<IPagingExecutionResult<GetAllEventQueryResponseDTO>>> Handle(GetAllEventQuery request, CancellationToken cancellationToken)
        {
            
            var events = await eventRepository.GetAllEventAsync(e => e.IsDeleted== false);
            if (events == null || events.Count == 0)
            {
                return new ServiceResponse<IPagingExecutionResult<GetAllEventQueryResponseDTO>> {

                    IsSuccess = true,
                    Data = await baseEntityRepository.GetPagedResult(
                    Enumerable.Empty<GetAllEventQueryResponseDTO>(),
                    pageSize: request.PageSize,
                    pageIndex: request.Page,
                    ordering: null,
                    cancellationToken: cancellationToken),
                    Message = await localizationService.Get(ValidationKeys.EventNotFound)
                };
            }


            var responses = events.Select(e => new GetAllEventQueryResponseDTO
            {

                CoverImageUrl = e.Images?
                    .OrderBy(i => i.Order)
                    .FirstOrDefault(i => i.IsCover)?.ImageUrl,
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Location = e.Location,
                Quota = e.Quota,
                ClubId = e.ClubId,  
                EventTag = e.EventTag,
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
