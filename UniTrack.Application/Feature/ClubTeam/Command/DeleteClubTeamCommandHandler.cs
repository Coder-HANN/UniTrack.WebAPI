using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.ClubTeam.Command
{
    public class DeleteClubTeamCommandHandler: IRequestHandler<DeleteClubTeamCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IClubTeamRepository clubTeamRepository;
        private readonly ILocalizationService localizationService;

        public DeleteClubTeamCommandHandler(
            ICurrentUserServices currentUserServices,
            IClubTeamRepository clubTeamRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.clubTeamRepository = clubTeamRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(DeleteClubTeamCommand request,CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();
            var role = currentUserServices.Role();

            if (clubId == null || role != Role.Club)
            {
                return ServiceResponse<string>.Fail(
                    await localizationService.Get(ValidationKeys.NotAuthorized));
            }

            var clubTeam = await clubTeamRepository.GetClubTeamId(request.ClubTeamId);

            if (clubTeam == null || clubTeam.ClubId != clubId.Value)
            {
                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.ClubTeamNotFound));
            }

            await clubTeamRepository.DeleteAsync(clubTeam);

            return ServiceResponse<string>.Success(await localizationService.Get(ValidationKeys.ClubTeamDeletedSuccess));
        }
    }
}
