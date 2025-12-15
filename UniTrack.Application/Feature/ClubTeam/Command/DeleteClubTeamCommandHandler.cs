using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.ClubTeam.Command
{
    public class DeleteClubTeamCommandHandler : IRequestHandler<DeleteClubTeamCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IClubTeamRepository clubTeamRepository;
        public DeleteClubTeamCommandHandler(
            ICurrentUserServices currentUserServices,
            IClubTeamRepository clubTeamRepository)
        {
            this.currentUserServices = currentUserServices;
            this.clubTeamRepository = clubTeamRepository;
        }
        public async Task<ServiceResponse<string>> Handle(DeleteClubTeamCommand request, CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();
            var role = currentUserServices.Role();

            if (clubId == null || role != Role.Club)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "You are not authorized to delete a team from this club.",
                    Data = null
                };
            }

            var clubTeam = await clubTeamRepository.GetClubTeamId(request.ClubTeamId);

            if (clubTeam == null || clubTeam.ClubId != clubId.Value)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "Club team not found or you are not authorized to delete this team.",
                    Data = null
                };
            }
            await clubTeamRepository.DeleteAsync(clubTeam);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = "Club team deleted successfully.",
                Data = null
            };
        }
    }
}
