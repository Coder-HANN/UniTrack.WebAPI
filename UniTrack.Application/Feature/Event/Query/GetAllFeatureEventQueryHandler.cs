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
    public class GetAllFeatureEventQueryHandler : IRequestHandler<GetAllFeatureEventQuery, ServiceResponse<IPagingExecutionResult<GetAllFeatureEventQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IBaseEntityRepository<Domain.Entities.Event> baseEntityRepository;
        private readonly IEventRepository eventRepository;
        private readonly ILocalizationService localizationService;
        private readonly IEventImageRepository eventImageRepository;
        private readonly IUserClubRepository userClubRepository;
        private readonly IEventUserRepository eventUserRepository;

        public GetAllFeatureEventQueryHandler(
            ICurrentUserServices currentUserServices,
            IBaseEntityRepository<Domain.Entities.Event> baseEntityRepository,
            IEventRepository eventRepository,
            ILocalizationService localizationService,
            IEventImageRepository eventImageRepository,
            IUserClubRepository userClubRepository,
            IEventUserRepository eventUserRepository)
        {
            this.currentUserServices = currentUserServices;
            this.baseEntityRepository = baseEntityRepository;
            this.eventRepository = eventRepository;
            this.localizationService = localizationService;
            this.eventImageRepository = eventImageRepository;
            this.userClubRepository = userClubRepository;
            this.eventUserRepository = eventUserRepository;
        }

        public async Task<ServiceResponse<IPagingExecutionResult<GetAllFeatureEventQueryResponseDTO>>> Handle(
            GetAllFeatureEventQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();

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

            // FIX 1: userId null ise boş liste kullan, .Value çağrısından kaçın
            var followClubs = userId.HasValue
                ? await userClubRepository.GetFollowedClubsByUserIdAsync(userId.Value)
                : new List<UniTrack.Domain.Entities.UserClub>();

            var followedClubIds = followClubs
                .Select(uc => uc.ClubId)
                .ToHashSet();

            var eventIds = events.Select(e => e.Id).ToList();

            var images = await eventImageRepository.GetByEventIdsAsync(eventIds);

            var imageLookup = images
                .Where(i => i.IsCover)
                .GroupBy(i => i.EventId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(i => i.Order)
                          .Select(i => i.ImageUrl)
                          .ToArray()
                );

            var responses = events.Select(e => new GetAllFeatureEventQueryResponseDTO
            {
                EventId = e.Id,
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                CityId = e.CityId,
                UniversityId = e.UniversityId,
                Location = e.Location,
                Quota = e.Quota,
                ClubId = e.ClubId,

                // FIX 2: ClubName'i takip listesinden değil, doğrudan Club navigation property'den al
                // GetFeatureEventsAsync() içinde .Include(e => e.Club) olmalı
                ClubName = e.Club?.Name ?? "Bilinmeyen Kulüp",

                EventTag = e.EventTag,
                Time = e.Time,
                Status = e.Status,
                CoverImageUrls = imageLookup.TryGetValue(e.Id, out var urls)
                    ? urls
                    : Array.Empty<string>(),
                IsJoin = userId.HasValue && e.EventUsers.Any()
                    ? e.EventUsers.Any(eu => eu.UserId == userId.Value && eu.IsJoined)
                    : false,
                Rate = e.EventUsers.Count > 0
                    ? ((float)e.EventUsers.Count(eu => eu.IsJoined) / e.Quota) * 100
                    : 0
            }).ToList();

            // FIX 3: Sıralama tek yerde yapılıyor, GetPagedResult'a ordering verilmiyor
            // Böylece takip edilen kulüp önceliklendirmesi ezilmiyor
            if (userId.HasValue)
            {
                responses = responses
                    .OrderByDescending(e => followedClubIds.Contains(e.ClubId))
                    .ThenByDescending(e => e.StartDate)
                    .ToList();
            }
            else
            {
                responses = responses
                    .OrderByDescending(e => e.StartDate)
                    .ToList();
            }

            var result = await baseEntityRepository.GetPagedResult(
                responses,
                pageSize: request.PageSize,
                pageIndex: request.Page,
                ordering: null, // FIX 3: null — sıralama zaten yukarıda yapıldı
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