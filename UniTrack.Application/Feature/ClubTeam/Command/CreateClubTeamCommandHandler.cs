using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.ClubTeam.Command
{
    public class CreateClubTeamCommandHandler : IRequestHandler<CreateClubTeamCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IClubTeamRepository clubTeamRepository;
        private readonly IUserClubRepository userClubRepository;
        public CreateClubTeamCommandHandler(
            ICurrentUserServices currentUserServices,
            IClubTeamRepository clubTeamRepository,
            IUserClubRepository userClubRepository)
        {
            this.currentUserServices = currentUserServices;
            this.clubTeamRepository = clubTeamRepository;
            this.userClubRepository = userClubRepository;
        }
        public async Task<ServiceResponse<string>> Handle(CreateClubTeamCommand request, CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();
            var role = currentUserServices.Role();

            if (clubId == null || clubId != request.ClubId || role!=Role.Club)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "You are not authorized to add a team to this club.",
                    Data = null
                };
            }

            var followingClub = await userClubRepository.GetClubFollowersByUserIdAsync(clubId.Value, request.UserDetailId);

            if (followingClub == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "The user must follow the club to be added to the team.",
                    Data = null
                };
            }

            var clubTeam = new Domain.Entities.ClubTeam
            {
                ClubId = request.ClubId,
                UserDetailId = request.UserDetailId,
                Title = request.Title
            };

            await clubTeamRepository.AddAsync(clubTeam);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = "Club team created successfully.",
                Data = null
            };
        }
    }
}
