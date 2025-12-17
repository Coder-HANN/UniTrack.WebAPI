using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Club;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetFollowClubQueryHandler : IRequestHandler<GetFollowClubQuery, ServiceResponse<IPagingExecutionResult<GetFollowClubQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IClubRepository clubRepository;
        private readonly IBaseEntityRepository<UserClub> baseEntityRepository;
        private readonly ILocalizationService localizationService;
        public GetFollowClubQueryHandler(
            ICurrentUserServices currentUserServices,
            IClubRepository clubRepository,
            IBaseEntityRepository<UserClub> baseEntityRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.clubRepository = clubRepository;
            this.baseEntityRepository = baseEntityRepository;
            this.localizationService = localizationService;
        }
        // TO DO: Kullanıcı panelinde olmalı 
        public async Task<ServiceResponse<IPagingExecutionResult<GetFollowClubQueryResponseDTO>>> Handle(GetFollowClubQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return new ServiceResponse<IPagingExecutionResult<GetFollowClubQueryResponseDTO>> {
                   
                        IsSuccess = false,
                        Data = null,
                        Message = await localizationService.Get(ValidationKeys.NotAuthorized)

                };
            }
            var role = currentUserServices.Role();
            if (role == null || role == Role.Club)
            {
                return new ServiceResponse<IPagingExecutionResult<GetFollowClubQueryResponseDTO>>
                { 

                         IsSuccess = false,
                         Data = null,
                         Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            var clubs = clubRepository.GetAllClubAsync(c => c.UserClubs.Any(cu => cu.UserId == userId)); // TO DO: club kontrolünü düzenle

            if (clubs == null)
            {
                return new ServiceResponse<IPagingExecutionResult<GetFollowClubQueryResponseDTO>> {
                   
                        IsSuccess = false,
                        Data = null,
                        Message = await localizationService.Get(ValidationKeys.ClubNotFound)

                };
            }

            var responses = clubs.Result.Select(c => new GetFollowClubQueryResponseDTO
            {
                Logo = c.Logo,
                CoverImage = c.CoverImage,
                Name = c.Name,
                Description = c.Description,
                ContactMail = c.ContectEmail,
                Followers = c.Follower,
                President = c.President,
                Tag = c.Tag
            });

            var result = await baseEntityRepository.GetPagedResult(
              responses,
              pageSize: request.PageSize,
              pageIndex: request.Page,
              ordering: q => q.OrderByDescending(x => x.Id),
              cancellationToken: cancellationToken
            );

            return new ServiceResponse<IPagingExecutionResult<GetFollowClubQueryResponseDTO>>
            {
                IsSuccess = true,
                Data = result,
                Message = null
            };
        }
    }
}
