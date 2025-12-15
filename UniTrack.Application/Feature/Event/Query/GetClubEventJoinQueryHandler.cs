using MediatR; 
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Ban;
using UniTrack.Application.DTOs.Comment;
using UniTrack.Application.DTOs.Event;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetClubEventJoinQueryHandler : IRequestHandler<GetClubEventJoinQuery, ServiceResponse<List<GetClubEventJoinQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventUserRepository eventUserRepository;

        public GetClubEventJoinQueryHandler(
            ICurrentUserServices currentUserServices,
            IEventUserRepository eventUserRepository)
        {
            this.currentUserServices = currentUserServices;
            this.eventUserRepository = eventUserRepository;
        }

        // Etkinliğe katılan kullanıcıları getir
        public async Task<ServiceResponse<List<GetClubEventJoinQueryResponseDTO>>> Handle(GetClubEventJoinQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            var clubId = currentUserServices.CurrentClub();
            if (userId == null && clubId == null)
            {
                return new ServiceResponse<List<GetClubEventJoinQueryResponseDTO>>
                {
                    
                        IsSuccess = false,
                        Data = null,
                        Message = "Unauthorized"
                    
                };
            }
            var role = currentUserServices.Role();
            if (role == null || role == Role.User)
            {
                return new ServiceResponse<List<GetClubEventJoinQueryResponseDTO>>
                { 
                         IsSuccess = false,
                         Data = null,
                         Message = "Yetkisiz kullanıcı"
                     
                };
            }

            var events = await eventUserRepository.GetClubEventJoinsByClubIdAsync(request.EventId);
            if (events == null)
            {
                return new ServiceResponse<List<GetClubEventJoinQueryResponseDTO>> {
                    
                        IsSuccess = false,
                        Data = null,
                        Message = "Etkinliğe katılan kullanıcı yok"
                    
                };
            }

            var responseList = events.Select(eu => new GetClubEventJoinQueryResponseDTO
            {
                Name = eu.User.UserDetail.Name,
                Surname = eu.User.UserDetail.Surname,
                Department = eu.User.UserDetail.Department.ToString(),
                UniversityId = eu.User.UserDetail.UniverstiyId,

            }).ToList();

            return new ServiceResponse<List<GetClubEventJoinQueryResponseDTO>> {
                
                    IsSuccess = true,
                    Data = responseList,
                    Message = "Etkinliğe katılan kullanıcılar başarıyla getirildi"
                
            };
        }
    }
}
