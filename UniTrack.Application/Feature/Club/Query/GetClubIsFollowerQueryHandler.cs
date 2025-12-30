using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Club;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetClubIsFollowerQueryHandler : IRequestHandler<GetClubIsFollowerQuery, ServiceResponse<List<GetClubIsFollowerQueryResponseDTO>>>
    {
        private readonly IUserClubRepository UserClubRepository;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;

        public GetClubIsFollowerQueryHandler(
            IUserClubRepository clubRepository,
            ICurrentUserServices currentUserService,
            ILocalizationService localizationService
            )
        {
            this.UserClubRepository = clubRepository;
            this.currentUserServices = currentUserService;
            this.localizationService = localizationService;
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
                        Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                    
                };
            }
            var role = currentUserServices.Role();
            if (role == null || role == Role.User)
            {
                return new ServiceResponse<List<GetClubIsFollowerQueryResponseDTO>> 
                {
                        
                        IsSuccess = false,
                        Data = null,
                        Message = await localizationService.Get(ValidationKeys.NotAuthorized)


                };
            }


            var followClub = await UserClubRepository.GetClubFollowersAsync(request.ClubId);

            if (followClub == null || followClub.Count == 0)
            {
                return new ServiceResponse<List<GetClubIsFollowerQueryResponseDTO>> 
                {
                        
                        IsSuccess = false,
                        Data = null,
                        Message = await localizationService.Get(ValidationKeys.ClubFansNotFound)


                };
            }

            var responses = followClub.Select(fc => new GetClubIsFollowerQueryResponseDTO
            {
               
                    Name = fc.User.UserDetail.Name,
                    Surname = fc.User.UserDetail.Surname,
                    Department = fc.User.UserDetail.Department.Name,
                    UniversityId = fc.User.UserDetail.UniverstiyId,
                    ImageUrl = fc.User.UserDetail.ProfileImageUrl  // Kontrol et
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
