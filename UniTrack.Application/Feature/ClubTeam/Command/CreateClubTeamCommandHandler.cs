using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.ClubTeam.Command
{
    public class CreateClubTeamCommandHandler: IRequestHandler<CreateClubTeamCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IClubTeamRepository clubTeamRepository;
        private readonly IUserClubRepository userClubRepository;
        private readonly ILocalizationService localizationService;

        public CreateClubTeamCommandHandler(
            ICurrentUserServices currentUserServices,
            IClubTeamRepository clubTeamRepository,
            IUserClubRepository userClubRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.clubTeamRepository = clubTeamRepository;
            this.userClubRepository = userClubRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(CreateClubTeamCommand request,CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();
            var role = currentUserServices.Role();

            if (clubId == null || role != Role.Club || clubId != request.ClubId)
            {
                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));
            }

            var isFollowing = await userClubRepository.GetClubFollowersByUserIdAsync(clubId.Value, request.UserDetailId);

            if (isFollowing == null)
            {
                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.UserMustFollowClub));
            }

            var clubTeam = new Domain.Entities.ClubTeam
            {
                ClubId = request.ClubId,
                UserDetailId = request.UserDetailId,
                Title = request.Title
            };

            await clubTeamRepository.AddAsync(clubTeam);

            return ServiceResponse<string>.Success(await localizationService.Get(ValidationKeys.ClubTeamCreatedSuccess));
        }
    }
}
