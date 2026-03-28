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
    public class GetAllClubForGuestQueryHandler : IRequestHandler<GetAllClubForGuestQuery, ServiceResponse<IPagingExecutionResult<GetAllClubForGuestQueryResponseDTO>>>
    {
        private readonly IClubRepository clubRepository;
        private readonly IBaseEntityRepository<Domain.Entities.Club> baseEntityRepository;
        private readonly ILocalizationService localizationService;

        public GetAllClubForGuestQueryHandler(
            IClubRepository clubRepository,
            IBaseEntityRepository<Domain.Entities.Club> baseEntityRepository,
            ILocalizationService localizationService)
        {
            this.clubRepository = clubRepository;
            this.baseEntityRepository = baseEntityRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<IPagingExecutionResult<GetAllClubForGuestQueryResponseDTO>>> Handle(GetAllClubForGuestQuery request, CancellationToken cancellationToken)
        {
            var clubs = await clubRepository.GetAllClubListAsync();
            if (clubs == null || clubs.Count == 0)
            {
                return new ServiceResponse<IPagingExecutionResult<GetAllClubForGuestQueryResponseDTO>>
                {
                    IsSuccess = true,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.ClubFansNotFound)
                };
            }

            var responses = clubs.Select(c => new GetAllClubForGuestQueryResponseDTO
            {
                Id = c.Id,
                LogoUrl = c.LogoUrl,
                CoverImageUrl = c.CoverImageUrl,
                Name = c.Name,
                Description = c.Description,
                ContactMail = c.ContectEmail,
                Followers = c.Follower,
                President = c.President,
                Tag = c.Tag,
                UniversityName = c.University.Name,
                City = c.City.Name,
            });

            var result = await baseEntityRepository.GetPagedResult(
                responses,
                pageSize: request.PageSize,
                pageIndex: request.Page,
                ordering: q => q.OrderByDescending(x => x.Id),
                cancellationToken: cancellationToken
            );

            return new ServiceResponse<IPagingExecutionResult<GetAllClubForGuestQueryResponseDTO>>
            {
                IsSuccess = true,
                Data = result,
                Message = null
            };
        }
    }
}
