using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Ban.Command
{
    public class BanForUserCommandHandler : IRequestHandler<BanForUserCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUserRepository userRepository;
        private readonly IBanRepository banRepository;
        private readonly ILocalizationService localizationService;
        public BanForUserCommandHandler(
            ICurrentUserServices currentUserServices,
            IUserRepository userRepository,
            IBanRepository banRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.userRepository = userRepository;
            this.banRepository = banRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(BanForUserCommand request, CancellationToken cancellationToken)
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
            if (role == null || role == Role.Club || role == Role.User)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized),
                };
            }
            var user = await userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.UserNotFound),
                    Data = null
                };
            }


            if (user.UserDetail.UniverstiyId != currentUserServices.UniversityId())
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized),
                    Data = null
                };
            }

            await banRepository.AddAsync(new Domain.Entities.Ban
            {
                UserId = request.UserId,
                LastDate = request.LastDate,
                IsBanned = true,
                Role = Role.User,
                Description = request.Description
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
