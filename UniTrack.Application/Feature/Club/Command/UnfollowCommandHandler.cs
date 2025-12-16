using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Club.Command
{
    public class UnfollowCommandHandler: IRequestHandler<UnfollowClubCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUserClubRepository userClubRepository;
        private readonly IClubRepository clubRepository;
        private readonly ILocalizationService localizationService;

        public UnfollowCommandHandler(
            ICurrentUserServices currentUserServices,
            IUserClubRepository userClubRepository,
            IClubRepository clubRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.userClubRepository = userClubRepository;
            this.clubRepository = clubRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(UnfollowClubCommand request,CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            var role = currentUserServices.Role();

            if (userId == null || role == null || role == Role.Club)
            {
                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));
            }

            var userClub = await userClubRepository.GetAsync(cu =>
                cu.ClubId == request.ClubId &&
                cu.UserId == userId.Value &&
                cu.IsFollowing);

            if (userClub == null)
            {
                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.NotFollowingClub));
            }

            await userClubRepository.DeleteAsync(userClub);

            var club = await clubRepository.GetAsync(c => c.Id == request.ClubId);

            club.Follower--;
            
            await clubRepository.UpdateAsync(club);

            return ServiceResponse<string>.Success(await localizationService.Get(ValidationKeys.UnfollowClubSuccess));
        }
    }
}
