using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Club.Command
{
    public class FollowClubCommandHandler : IRequestHandler<FollowClubCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUserClubRepository UserClubRepository;
        private readonly IClubRepository clubRepository;
        private readonly ILocalizationService localizationService;

        public FollowClubCommandHandler(ICurrentUserServices currentUserServices,
            IUserClubRepository UserClubRepository,
            IClubRepository clubRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.UserClubRepository = UserClubRepository;
            this.clubRepository = clubRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(FollowClubCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            var role = currentUserServices.Role();

            if (userId == null || role == null || role == Role.Club)
            {
                return ServiceResponse<string>.Fail(
                    await localizationService.Get(ValidationKeys.NotAuthorized));
            }

            var existingEntry = await UserClubRepository
                .GetAsync(cu => cu.ClubId == request.ClubId && cu.UserId == userId.Value);

            if (existingEntry != null)
            {
                return ServiceResponse<string>.Fail(
                    await localizationService.Get(ValidationKeys.AlreadyFollowingClub));
            }

            var userClub = new UserClub
            {
                ClubId = request.ClubId,
                UserId = userId.Value,
                IsFollowing = true
            };

            await UserClubRepository.AddAsync(userClub);

            var club = await clubRepository.GetAsync(c => c.Id == request.ClubId);
            club.Follower++;
            await clubRepository.UpdateAsync(club);

            return ServiceResponse<string>.Success(
                await localizationService.Get(ValidationKeys.FollowClubSuccess));
        }

    }
}

