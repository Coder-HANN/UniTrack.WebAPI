using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Profile;

namespace UniTrack.Application.Feature.Profile.Query
{
    public class GetAdminProfileQueryHadnler : IRequestHandler<GetAdminProfileQuery, ServiceResponse<AdminProfileUpdateResponseDTO>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUserRepository userRepository;
        private readonly ILocalizationService localizationService;

        public GetAdminProfileQueryHadnler(
            ICurrentUserServices currentUserServices,
            IUserRepository userRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.userRepository = userRepository;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<AdminProfileUpdateResponseDTO>> Handle(GetAdminProfileQuery query, CancellationToken cancellationToken)
        {
            var adminId = currentUserServices.CurrentUser();
            if (adminId == null)
            {
                return new ServiceResponse<AdminProfileUpdateResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            var user = await userRepository.GetByIdAsync(adminId.Value);

            if (user == null)
            {
                return new ServiceResponse<AdminProfileUpdateResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.UserNotFound)
                };
            }

            var adminProfile = new AdminProfileUpdateResponseDTO
            {
                Name = user.UserDetail.Name,
                Email = user.Email,
                UniverstiyId = user.UserDetail.UniverstiyId,
                IsNotified = user.UserDetail.IsNotified,
                ProfileImageUrl = user.UserDetail.ProfileImageUrl,
                Password = null
            };

            return ServiceResponse<AdminProfileUpdateResponseDTO>.Success(null, adminProfile);
        }
    }
}
