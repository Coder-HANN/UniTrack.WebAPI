using MediatR;
using Microsoft.AspNetCore.SignalR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Ban.Command
{
    public class BanForClubCommandHandler : IRequestHandler<BanForClubCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IClubRepository clubRepository;
        private readonly IBanRepository banRepository;
        private readonly ILocalizationService localizationService;
        public BanForClubCommandHandler(
            ICurrentUserServices currentUserServices,
            IClubRepository clubRepository,
            IBanRepository banRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.clubRepository = clubRepository;
            this.banRepository = banRepository;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<string>> Handle(BanForClubCommand request, CancellationToken cancellationToken)
        {
            var adminId = currentUserServices.CurrentUser();
            if (adminId == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized),
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
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized),
                };
            }

            var club = await clubRepository.GetByIdAsync(request.ClubId);
            if (club == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.ClubNotFound),
                    Data = null
                };
            }


            if (club.UniversityId != currentUserServices.UniversityId())
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized),
                    Data = null
                };
            }
            var ban = await banRepository.GetActiveBanForClubAsync(request.ClubId);

            if (ban != null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.ClubAlreadyBanned),
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
                CreatedDate = DateTimeOffset.UtcNow,

            });

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = await localizationService.Get(ValidationKeys.OperationSuccessful),
                Data = null
            };

        }
    }
}
