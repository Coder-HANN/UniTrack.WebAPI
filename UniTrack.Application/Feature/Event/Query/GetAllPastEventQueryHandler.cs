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
    public class GetAllPastEventQueryHandler : IRequestHandler<GetAllPastEventQuery, ServiceResponse<IPagingExecutionResult<GetAllPastEventQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IBaseEntityRepository<Domain.Entities.Event> baseEntityRepository;
        private readonly IEventRepository eventRepository;
        private readonly ILocalizationService localizationService;
        public GetAllPastEventQueryHandler(
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
        public async Task<ServiceResponse<IPagingExecutionResult<GetAllPastEventQueryResponseDTO>>> Handle(GetAllPastEventQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
          
            var events = await eventRepository.GetPastEventsAsync();

            if (events == null || events.Count == 0)
            {
                return new ServiceResponse<IPagingExecutionResult<GetAllPastEventQueryResponseDTO>>
                {
                    IsSuccess = true,
                    Data = await baseEntityRepository.GetPagedResult(
                    Enumerable.Empty<GetAllPastEventQueryResponseDTO>(),
                    pageSize: request.PageSize,
                    pageIndex: request.Page,
                    ordering: null,
                    cancellationToken: cancellationToken),
                    Message = await localizationService.Get(ValidationKeys.EventNotFound)
                };
            }

            var responses = events.Select(e => new GetAllPastEventQueryResponseDTO
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
                ClubName = e.Club.Name,
                EventTag = e.EventTag,
                Time = e.Time,
                Status = e.Status,
                Rate = e.EventUsers.Count > 0 ? ((float)e.EventUsers.Count(eu => eu.IsJoined) / e.Quota) * 100 : 0,
                CityId = e.CityId,
                UniversityId = e.UniversityId,
                EventId = e.IdClient 
            }).ToList();

            if (userId != null)
            {
                // Kullanıcının takip ettiği kulüp Id'lerini al
                var followedClubIds = events
                    .SelectMany(e => e.Club.UserClubs)
                    .Where(uc => uc.UserId == userId.Value)
                    .Select(uc => uc.ClubId)
                    .Distinct()
                    .ToList();

                // Önce takip edilen kulüplerin etkinlikleri, sonra tarih sırası
                responses = responses
                    .OrderByDescending(e => followedClubIds.Contains(e.ClubId))
                    .ThenByDescending(e => e.StartDate)
                    .ToList();
            }
            else
            {
                // Misafir kullanıcılar sadece tarih sırasına göre
                responses = responses
                    .OrderByDescending(e => e.StartDate)
                    .ToList();
            }

            var result = await baseEntityRepository.GetPagedResult(
              responses,
              pageSize: request.PageSize,
              pageIndex: request.Page,
              ordering: q => q.OrderByDescending(x => x.StartDate),
              cancellationToken: cancellationToken
            );

            return new ServiceResponse<IPagingExecutionResult<GetAllPastEventQueryResponseDTO>>
            {
                IsSuccess = true,
                Data = result,
                Message = await localizationService.Get(ValidationKeys.GetEventSuccesses)
            };
        }
    }
}
