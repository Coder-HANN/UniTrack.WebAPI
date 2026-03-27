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
    public class GetClubEventQueryHandler : IRequestHandler<GetClubEventQuery, ServiceResponse<IPagingExecutionResult<GetClubEventQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventRepository eventRepository;
        private readonly ILocalizationService localizationService;
        private readonly IBaseEntityRepository<Domain.Entities.Event> baseEntityRepository;

        public GetClubEventQueryHandler(
            ICurrentUserServices currentUserServices,
            IEventRepository eventRepository,
            ILocalizationService localizationService,
            IBaseEntityRepository<Domain.Entities.Event> baseEntityRepository)
        {
            this.currentUserServices = currentUserServices;
            this.eventRepository = eventRepository;
            this.localizationService = localizationService;
            this.baseEntityRepository = baseEntityRepository;
        }

        public async Task<ServiceResponse<IPagingExecutionResult<GetClubEventQueryResponseDTO>>> Handle(GetClubEventQuery request, CancellationToken cancellationToken)
        {

            var events = await eventRepository.GetAllClubEventAsync(e => e.ClubId == request.ClubId && e.IsDeleted == false);
            if (events == null || events.Count == 0)
            {
                return new ServiceResponse<IPagingExecutionResult<GetClubEventQueryResponseDTO>> {
                   
                        IsSuccess = true,
                        Data = await baseEntityRepository.GetPagedResult(
                        Enumerable.Empty<GetClubEventQueryResponseDTO>(),
                        pageSize: request.PageSize,
                        pageIndex: request.Page,
                        ordering: null,
                        cancellationToken: cancellationToken),
                    Message = await localizationService.Get(ValidationKeys.EventNotFound)
                };
            }

            var response = events.Select(e => new GetClubEventQueryResponseDTO
            {
                    CoverImageUrl = e.Images?
                    .OrderBy(i => i.Order)
                    .FirstOrDefault(i => i.IsCover)?.ImageUrl,

                    Title = e.Title,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    Location = e.Location,
                    Quota = e.Quota,
                    Time=e.Time,
                    ClubId = e.ClubId,
                    Status = e.Status,
                    EventTag = e.EventTag,
                    Rate = e.EventUsers.Count > 0 ? ((float)e.EventUsers.Count(eu => eu.IsJoined) / e.Quota) * 100 : 0,
                    EventId = e.Id,
                    JoinedCount = e.EventUsers?.Count(eu => eu.IsJoined) ?? 0,

            }).ToList();

            var result = await baseEntityRepository.GetPagedResult(
             response,
             pageSize: request.PageSize,
             pageIndex: request.Page,
             ordering: q => q.OrderByDescending(x => x.StartDate),
             cancellationToken: cancellationToken
           );

            return new ServiceResponse<IPagingExecutionResult<GetClubEventQueryResponseDTO>> {
                
                    IsSuccess = true,
                    Data = result,
                    Message = null
            };
        }
    }
}
