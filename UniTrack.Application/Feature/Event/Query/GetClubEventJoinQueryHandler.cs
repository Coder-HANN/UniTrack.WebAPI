using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetClubEventJoinQueryHandler : IRequestHandler<GetClubEventJoinQuery, ServiceResponse<List<GetClubEventJoinQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventUserRepository eventUserRepository;
        private readonly ILocalizationService localizationService;

        public GetClubEventJoinQueryHandler(
            ICurrentUserServices currentUserServices,
            IEventUserRepository eventUserRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.eventUserRepository = eventUserRepository;
            this.localizationService = localizationService;
        }

        // Etkinliğe katılan kullanıcıları getir
        public async Task<ServiceResponse<List<GetClubEventJoinQueryResponseDTO>>> Handle(GetClubEventJoinQuery request, CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();
            if (clubId == null)
            {
                return new ServiceResponse<List<GetClubEventJoinQueryResponseDTO>>
                {
                    
                        IsSuccess = false,
                        Data = null,
                        Message = await localizationService.Get(ValidationKeys.NotAuthorized)

                };
            }
            var role = currentUserServices.Role();
            if (role == null || role == Role.User)
            {
                return new ServiceResponse<List<GetClubEventJoinQueryResponseDTO>>
                { 
                         IsSuccess = false,
                         Data = null,
                         Message = await localizationService.Get(ValidationKeys.NotAuthorized)

                };
            }

            var events = await eventUserRepository.GetClubEventJoinsByClubIdAsync(request.EventId);
            if (events == null)
            {
                return new ServiceResponse<List<GetClubEventJoinQueryResponseDTO>> {
                    
                        IsSuccess = true,
                        Data = null,
                        Message = await localizationService.Get(ValidationKeys.UserNotFound)

                };
            }

            var responseList = events.Select(eu => new GetClubEventJoinQueryResponseDTO
            {
                Name = eu.User.UserDetail.Name,
                Surname = eu.User.UserDetail.Surname,
                Department = eu.User.UserDetail.Department.Name,
                UniversityName = eu.User.UserDetail.University.Name,
                Email = eu.User.Email

            }).ToList();

            return new ServiceResponse<List<GetClubEventJoinQueryResponseDTO>> {
                
                    IsSuccess = true,
                    Data = responseList,
                    Message = null
                
            };
        }
    }
}
