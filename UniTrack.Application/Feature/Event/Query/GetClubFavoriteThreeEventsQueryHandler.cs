// UniTrack.Application.Feature.Event.Query/GetClubFavoriteThreeEventsQueryHandler.cs

using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetClubFavoriteThreeEventsQueryHandler : 
        IRequestHandler<GetClubFavoriteThreeEventsQuery, ServiceResponse<List<FavoriteEventsResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventRepository eventRepository;
        private readonly ICommentRepository commentRepository; // Geri Eklendi

        public GetClubFavoriteThreeEventsQueryHandler(
            ICurrentUserServices currentUserServices, 
            IEventRepository eventRepository,
            ICommentRepository commentRepository) // DI'a Eklendi
        {
            this.currentUserServices = currentUserServices;
            this.eventRepository = eventRepository;
            this.commentRepository = commentRepository;
        }

        public async Task<ServiceResponse<List<FavoriteEventsResponseDTO>>> Handle(
            GetClubFavoriteThreeEventsQuery request, 
            CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();
            var userId = currentUserServices.CurrentUser();

            // 1. Yetkilendirme Kontrolü
            if (clubId == null || clubId != request.ClubId || userId == null)
            {
                return new ServiceResponse<List<FavoriteEventsResponseDTO>>
                {
                    IsSuccess = false,
                    Message = "Unauthorized access to club favorite events.",
                    Data = null

                };
            }
            
            // 2. En çok katılım alan ilk 3 etkinlik çekiliyor
            var favoriteEvents = await eventRepository.GetTopThreeFavoriteEventsByClubIdAsync(request.ClubId);
            
            if (favoriteEvents == null || favoriteEvents.Count == 0)
            {
                return new ServiceResponse<List<FavoriteEventsResponseDTO>>
                    {
                        IsSuccess = false,
                        Message = "No favorite events found for the club.",
                        Data = null
                    }
                ;
            }
            
            var favoriteEventsId = favoriteEvents.Select(e => e.Id).ToList();

            // 3. Performanslı Puan ve Yorum Sayısı Çekme (Tek sorgu)
            var ratingsDict = await commentRepository.GetEventsRatingsSummaryAsync(favoriteEventsId);

            
            // 4. DTO'ya dönüştürme ve verileri birleştirme
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
                Message = "Favorite events retrieved successfully.",
                Data = favoriteEventsDto
            };
        }
    }
}