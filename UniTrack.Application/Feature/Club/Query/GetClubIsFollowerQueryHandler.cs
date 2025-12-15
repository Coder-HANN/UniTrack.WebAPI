using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Club;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetClubIsFollowerQueryHandler : IRequestHandler<GetClubIsFollowerQuery, ServiceResponse<List<GetClubIsFollowerQueryResponseDTO>>>
    {
        private readonly IUserClubRepository UserClubRepository;
        private readonly ICurrentUserServices currentUserServices;

        public GetClubIsFollowerQueryHandler(
            IUserClubRepository clubRepository,
            ICurrentUserServices currentUserService
            )
        {
            this.UserClubRepository = clubRepository;
            this.currentUserServices = currentUserService;
        }
        // Kulübü takip eden kullanıcıları getir
        public async Task<ServiceResponse<List<GetClubIsFollowerQueryResponseDTO>>> Handle(GetClubIsFollowerQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentClub();
            var clubId = currentUserServices.CurrentUser();
            if (userId == null && clubId == null)
            {
                return new ServiceResponse<List<GetClubIsFollowerQueryResponseDTO>>{
                   
                        IsSuccess = false,
                        Data = null,
                        Message = "Unauthorized"
                    
                };
            }
            var role = currentUserServices.Role();
            if (role == null || role == Role.User)
            {
                return new ServiceResponse<List<GetClubIsFollowerQueryResponseDTO>> 
                {
                        
                        IsSuccess = false,
                        Data = null,
                        Message = "Yetkisiz kullanıcı"
                        
                    
                };
            }


            var followClub = await UserClubRepository.GetClubFollowersAsync(request.ClubId);

            if (followClub == null || followClub.Count == 0)
            {
                return new ServiceResponse<List<GetClubIsFollowerQueryResponseDTO>> 
                {
                        
                        IsSuccess = false,
                        Data = null,
                        Message = "No followers found for this club"
                        
                    
                };
            }

            var responses = followClub.Select(fc => new GetClubIsFollowerQueryResponseDTO
            {
               
                    Name = fc.User.UserDetail.Name,
                    Surname = fc.User.UserDetail.Surname,
                    Department = fc.User.UserDetail.Department.ToString(),
                    UniversityId = fc.User.UserDetail.UniverstiyId,
                    Image = fc.User.UserDetail.ProfileImage  // Kontrol et
            }).ToList();

            return new ServiceResponse<List<GetClubIsFollowerQueryResponseDTO>> 
            {    
                    IsSuccess = true,
                    Data = responses,
                    Message = null
            };
        }
    }
}
