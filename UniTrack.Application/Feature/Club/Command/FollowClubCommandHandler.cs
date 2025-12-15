using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Club.Command
{
    public class FollowClubCommandHandler : IRequestHandler<FollowClubCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUserClubRepository UserClubRepository;
        private readonly IClubRepository clubRepository;

        public FollowClubCommandHandler(ICurrentUserServices currentUserServices,
            IUserClubRepository UserClubRepository,
            IClubRepository clubRepository)
        {
            this.currentUserServices = currentUserServices;
            this.UserClubRepository = UserClubRepository;
            this.clubRepository = clubRepository;
        }

        public async Task<ServiceResponse<string>> Handle(FollowClubCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();

            if (userId == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Unauthorized"
                };
            }

            var role = currentUserServices.Role();

            if (role == null || role == Role.Club )
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Yetkisiz kullanıcı"
                };
            }

            var existingEntry = await UserClubRepository.GetAsync(cu => cu.ClubId == request.ClubId && cu.UserId == userId.Value);
            
            if (existingEntry != null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "You are already following this club"
                };
            }

            var UserClub = new UserClub
            {
                ClubId = request.ClubId,
                UserId = userId.Value,
                IsFollowing = true
            };

            await UserClubRepository.AddAsync(UserClub);

            var add = await clubRepository.GetAsync(c => c.Id == request.ClubId);

            add.Follower = add.Follower + 1;

            await clubRepository.UpdateAsync(add);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Data = null,
                Message = "Kayıt başarılı"
            };
        }
    }
}
