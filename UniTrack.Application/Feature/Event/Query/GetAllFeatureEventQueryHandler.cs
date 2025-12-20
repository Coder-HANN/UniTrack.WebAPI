using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Comment;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetAllFeatureEventQueryHandler : IRequestHandler<GetAllFeatureEventQuery, ServiceResponse<IPagingExecutionResult<GetAllFeatureEventQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IBaseEntityRepository<Domain.Entities.Event> baseEntityRepository;
        private readonly IEventRepository eventRepository;
        private readonly ILocalizationService localizationService;
        public GetAllFeatureEventQueryHandler(
            ICurrentUserServices currentUserServices,
            IBaseEntityRepository<Domain.Entities.Event> baseEntityRepository,
            IEventRepository eventRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.baseEntityRepository = baseEntityRepository;
            this.eventRepository = eventRepository;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<IPagingExecutionResult<GetAllFeatureEventQueryResponseDTO>>> Handle(GetAllFeatureEventQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
               return new ServiceResponse<IPagingExecutionResult<GetAllFeatureEventQueryResponseDTO>>
               {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
               };
            }
            var events = await eventRepository.GetFeatureEventsAsync();
            if (events == null || events.Count == 0)
            {
                return new ServiceResponse<IPagingExecutionResult<GetAllFeatureEventQueryResponseDTO>>
                {
                    IsSuccess = true,
                    Data = await baseEntityRepository.GetPagedResult(
                    Enumerable.Empty<GetAllFeatureEventQueryResponseDTO>(),
                    pageSize: request.PageSize,
                    pageIndex: request.Page,
                    ordering: null,
                    cancellationToken: cancellationToken),
                    Message = await localizationService.Get(ValidationKeys.EventNotFound)
                };
            }
            var responses = events.Select(e => new GetAllFeatureEventQueryResponseDTO
            {
                Image = e.Image,
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
            }).ToList();
            
            var result = await baseEntityRepository.GetPagedResult(
              responses,
              pageSize: request.PageSize,
              pageIndex: request.Page,
              ordering: q => q.OrderByDescending(x => x.StartDate),
              cancellationToken: cancellationToken
            );

            return new ServiceResponse<IPagingExecutionResult<GetAllFeatureEventQueryResponseDTO>>
            { 
                IsSuccess = true,
                Data = result,
                Message = await localizationService.Get(ValidationKeys.GetEventSuccesses)
            };
        }
    }
}
