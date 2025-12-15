using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Ban;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Ban.Query
{
    public class GetBanedClubQueryHandler : IRequestHandler<GetBanedClubQuery, ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IBanRepository banRepository;

        public GetBanedClubQueryHandler(ICurrentUserServices currentUserServices, IBanRepository banRepository)
        {
            this.currentUserServices = currentUserServices;
            this.banRepository = banRepository;
        }

        public async Task<ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>>> Handle(GetBanedClubQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return new ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>> {
                    
                        IsSuccess = false,
                        Data = null,
                        Message = "Unauthorizaton"
                    
                };
            }
            var role = currentUserServices.Role();
            if (role == null || role == Role.Club || role == Role.User)
            {
                return new ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>>
                { 

                        IsSuccess = false,
                        Data = null,
                        Message = "Yetkisiz kullanıcı"
                    
                        
                };
            }

            var list = await banRepository.GetAllAsync();
            if (list == null)
            {
                return new ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>> {
                    
                        IsSuccess = false,
                        Data = null,
                        Message = "Liste boş"
                    
                };
            }

            var responses = list.Select(b => new GetBanedClubOrUserQueryResponseDTO
            {
               
                    Id = b.Id,
                    Role = b.Club.Role,
                    ClubId = b.ClubId,
                    CreatedDate = b.CreatedDate,
                    LastDate = b.LastDate
                
            }).ToList();

            return new ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>>
            {
                
                    IsSuccess = true,
                    Data = responses,
                    Message = null
                
            };
        }
    }
}
