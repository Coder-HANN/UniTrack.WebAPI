using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetClubEventQueryHandler : IRequestHandler<GetClubEventQuery, ServiceResponse<List<GetClubEventQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventRepository eventRepository;
        private readonly ILocalizationService localizationService;

        public GetClubEventQueryHandler(
            ICurrentUserServices currentUserServices,
            IEventRepository eventRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.eventRepository = eventRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<List<GetClubEventQueryResponseDTO>>> Handle(GetClubEventQuery request, CancellationToken cancellationToken)
        {
          
            var events = await eventRepository.GetAllClubEventAsync(e => e.ClubId == request.ClubId);
            if(events == null || events.Count == 0)
            {
                return new ServiceResponse<List<GetClubEventQueryResponseDTO>> {
                   
                        IsSuccess = true,
                        Data = null,
                        Message = await localizationService.Get(ValidationKeys.EventNotFound)
                };
            }
            var response = events.Select(e => new GetClubEventQueryResponseDTO
            {
                    Image = e.Image,
                    Title = e.Title,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Clock = e.Clock,
                    Location = e.Location,
                    Quota = e.Quota,
                    Time=e.Time,
                    ClubId = e.ClubId,
                    Status = e.Status,
                    EventTag = e.EventTag,

            }).ToList();

            return new ServiceResponse<List<GetClubEventQueryResponseDTO>> {
                
                    IsSuccess = true,
                    Data = response,
                    Message = null
            };
        }
    }
}
