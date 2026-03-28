using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Club;

namespace UniTrack.Application.Feature.Club.Query
{
    public class SearchClubFollowerQueryHandler : IRequestHandler<SearchClubFollowerQuery, ServiceResponse<List<ClubFollowerSearchResponseDTO>>>
    {
        private readonly IUserClubRepository userClubRepository;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;

        public SearchClubFollowerQueryHandler(
            IUserClubRepository userClubRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService)
        {
            this.userClubRepository = userClubRepository;
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<List<ClubFollowerSearchResponseDTO>>> Handle(SearchClubFollowerQuery request,CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();
            if (clubId == null)
                return ServiceResponse<List<ClubFollowerSearchResponseDTO>>.Fail(
                    await localizationService.Get(ValidationKeys.NotAuthorized));

            var followers = await userClubRepository.SearchFollowersByNameAsync(clubId.Value, request.Name);

            var result = followers.Select(f => new ClubFollowerSearchResponseDTO
            {
                UserId = f.User.Id,
                FullName = $"{f.User.UserDetail.Name} {f.User.UserDetail.Surname}",
                ProfileImageUrl = f.User.UserDetail.ProfileImageUrl,
                Mail = f.User.Email
            }).ToList();

            return ServiceResponse<List<ClubFollowerSearchResponseDTO>>.Success(null, result);
        }
    }
}