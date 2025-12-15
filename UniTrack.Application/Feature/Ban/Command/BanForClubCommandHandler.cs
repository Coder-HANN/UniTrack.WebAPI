using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Ban.Command
{
    public class BanForClubCommandHandler : IRequestHandler<BanForClubCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IClubRepository clubRepository;
        private readonly IBanRepository banRepository;
        public BanForClubCommandHandler(
            ICurrentUserServices currentUserServices,
            IClubRepository clubRepository,
            IBanRepository banRepository)
        {
            this.currentUserServices = currentUserServices;
            this.clubRepository = clubRepository;
            this.banRepository = banRepository;
        }
        public async Task<ServiceResponse<string>> Handle(BanForClubCommand request, CancellationToken cancellationToken)
        {
            var adminId = currentUserServices.CurrentUser();
            if (adminId == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "Unauthorized",
                    Data = null
                };
            }
            var role = currentUserServices.Role();
            if (role == null|| role == Role.Club || role == Role.User)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Yetkisiz kullanıcı"
                };
            }

            var club = await clubRepository.GetByIdAsync(request.ClubId);
            if (club == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "Club not found",
                    Data = null
                };
            }

            await banRepository.AddAsync(new Domain.Entities.Ban
            {
                ClubId = request.ClubId,
                LastDate = request.LastDate,
                Role = Role.Club,
                IsBanned = true,
                Description = request.Description,
            });

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = "Club banned successfully",
                Data = null
            };

        }
    }
}
