using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Club;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetAllClubQueryHandler : IRequestHandler<GetAllClubQuery, ServiceResponse<IPagingExecutionResult<GetAllClubQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IClubRepository clubRepository;
        private readonly BaseEntityRepository<Domain.Entities.Club> baseEntityRepository;
        private readonly ILocalizationService localizationService;

        public GetAllClubQueryHandler(
            ICurrentUserServices currentUserServices,
            IClubRepository clubRepository,
            BaseEntityRepository<Domain.Entities.Club> baseEntityRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.clubRepository = clubRepository;
            this.baseEntityRepository = baseEntityRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<IPagingExecutionResult<GetAllClubQueryResponseDTO>>> Handle(GetAllClubQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            var clubId = currentUserServices.CurrentClub();
            if (userId == null && clubId == null)
            {
                return new ServiceResponse<IPagingExecutionResult<GetAllClubQueryResponseDTO>>
                {


                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)

                };
            }

            var clubs = await clubRepository.GetAllAsync();
            if (clubs == null || clubs.Count == 0)
            {
                return new ServiceResponse<IPagingExecutionResult<GetAllClubQueryResponseDTO>>{
 
                    
                        IsSuccess = false,
                        Data = null,
                        Message = await localizationService.Get(ValidationKeys.ClubFansNotFound)
                    
                };
            }

            var responses = clubs.Select(c => new GetAllClubQueryResponseDTO
            {
                Id = c.Id,
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

            return new ServiceResponse<IPagingExecutionResult<GetAllClubQueryResponseDTO>>
            {
                IsSuccess = true,
                Data = result,
                Message = null
            };
        }
    }
}
