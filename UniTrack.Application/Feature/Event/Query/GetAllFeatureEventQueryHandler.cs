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

            // Etkinliklere ait TÜM resimleri çekiyoruz (IsCover şartı olmadan!)
            var images = await eventImageRepository.GetByEventIdsAsync(eventIds);

            // Resimleri etkinlik Id'sine göre grupluyoruz (Artık IsCover filtresi yapmıyoruz, çünkü tüm resimler lazım)
            var imageLookup = images
                .GroupBy(i => i.EventId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(i => i.Order).ToList() // Resimleri sırasına göre listeledik
                );

            var responses = events.Select(e =>
            {
                // Bu etkinliğe ait resimleri sözlükten (dictionary) güvenli bir şekilde al
                var eventImages = imageLookup.TryGetValue(e.Id, out var imgs) ? imgs : new List<Domain.Entities.EventImage>();

                return new GetAllFeatureEventQueryResponseDTO
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
                    ClubName = e.Club?.Name ?? "Bilinmeyen Kulüp",
                    EventTag = e.EventTag,
                    Time = e.Time,
                    Status = e.Status,

                    // 1. Kapak Fotoğrafını Ayarla: IsCover true olanın URL'sini al, yoksa ilk resmi al, o da yoksa null bırak
                    CoverImageUrl = eventImages.FirstOrDefault(i => i.IsCover)?.ImageUrl
                                    ?? eventImages.FirstOrDefault()?.ImageUrl,

                    // 2. Tüm Resimleri Ayarla: Sadece URL'leri seç ve string listesine çevir
                    ImageUrls = eventImages.Select(i => i.ImageUrl).Where(url => !string.IsNullOrWhiteSpace(url)).ToList(),

                    IsJoin = userId.HasValue && e.EventUsers.Any()
                        ? e.EventUsers.Any(eu => eu.UserId == userId.Value && eu.IsJoined)
                        : false,
                    Rate = e.EventUsers.Count > 0
                        ? ((float)e.EventUsers.Count(eu => eu.IsJoined) / e.Quota) * 100
                        : 0
                };
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