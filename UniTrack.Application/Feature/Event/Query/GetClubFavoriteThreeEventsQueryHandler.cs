using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetClubFavoriteThreeEventsQueryHandler : IRequestHandler<GetClubFavoriteThreeEventsQuery, ServiceResponse<List<FavoriteEventsResponseDTO>>>
    {
        private readonly IEventRepository eventRepository;
        private readonly ICommentRepository commentRepository; 
        private readonly ILocalizationService localizationService;

        public GetClubFavoriteThreeEventsQueryHandler(
            IEventRepository eventRepository,
            ICommentRepository commentRepository,
            ILocalizationService localizationService)
        {
            this.eventRepository = eventRepository;
            this.commentRepository = commentRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<List<FavoriteEventsResponseDTO>>> Handle(GetClubFavoriteThreeEventsQuery request, CancellationToken cancellationToken)
        {

            // 1.En çok katılım alan ilk 3 etkinlik çekiliyor
            var favoriteEvents = await eventRepository.GetTopThreeFavoriteEventsByClubIdAsync(request.ClubId);
            
            if (favoriteEvents == null || favoriteEvents.Count == 0)
            {
                return new ServiceResponse<List<FavoriteEventsResponseDTO>>
                {
                    IsSuccess = true,
                    Message = await localizationService.Get(ValidationKeys.EventNotFound),
                    Data = null
                };
                
            }
            
            var favoriteEventsId = favoriteEvents.Select(e => e.Id).ToList();

            // 2. Performanslı Puan ve Yorum Sayısı Çekme (Tek sorgu)
            var ratingsDict = await commentRepository.GetEventsRatingsSummaryAsync(favoriteEventsId);

            
            // 3. DTO'ya dönüştürme ve verileri birleştirme
            var favoriteEventsDto = favoriteEvents.Select(e =>
            {
                // Puan özetini Dictionary'den al (bulunamazsa (0f, 0) varsayılan değer döner)
                var ratingSummary = ratingsDict.GetValueOrDefault(e.Id, (0f, 0)); 
                
                return new FavoriteEventsResponseDTO
                {
                    EventName = e.Title,
                    EventDate = e.StartDate,
                    EventLocation = e.Location,
                    EventImage = e.Image,
                    joinerCount = e.Joiner, 
                    Qouta = e.Quota,
                    Time = e.Time,

                    // Puan verileri isimlendirilmiş Tuple'dan atanıyor
                    Points = ratingSummary.Item1, 
                    PointsCount = ratingSummary.Item2
                };
            }).ToList();

            // 5. Başarılı Yanıt
            return new ServiceResponse<List<FavoriteEventsResponseDTO>>
            {
                IsSuccess = true,
                Message = null,
                Data = favoriteEventsDto
            };
        }
    }
}