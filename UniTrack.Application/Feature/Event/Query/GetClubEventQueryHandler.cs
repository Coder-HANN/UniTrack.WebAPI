using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetClubEventQueryHandler : IRequestHandler<GetClubEventQuery, ServiceResponse<List<GetClubEventQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventRepository eventRepository;

        public GetClubEventQueryHandler(
            ICurrentUserServices currentUserServices,
            IEventRepository eventRepository)
        {
            this.currentUserServices = currentUserServices;
            this.eventRepository = eventRepository;
        }

        public async Task<ServiceResponse<List<GetClubEventQueryResponseDTO>>> Handle(GetClubEventQuery request, CancellationToken cancellationToken)
        {
          
            var events = await eventRepository.GetAllClubEventAsync(e => e.ClubId == request.ClubId);
            if(events == null || events.Count == 0)
            {
                return new ServiceResponse<List<GetClubEventQueryResponseDTO>> {
                   
                        IsSuccess = false,
                        Data = null,
                        Message = "No events found for this club"
                    
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
                    Tags = e.Tag,

            }).ToList();

            return new ServiceResponse<List<GetClubEventQueryResponseDTO>> {
                
                    IsSuccess = true,
                    Data = response,
                    Message = "Club events retrieved successfully"
                
            };
        }
    }
}
