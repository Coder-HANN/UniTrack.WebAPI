using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Profile;

namespace UniTrack.Application.Feature.Profile.Query
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, ServiceResponse<UserProfileUpdateResponseDTO>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUserRepository userRepository;
        private readonly ILocalizationService localizationService;

        public GetUserProfileQueryHandler(
            ICurrentUserServices currentUserServices,
            IUserRepository userRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.userRepository = userRepository;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<UserProfileUpdateResponseDTO>> Handle(GetUserProfileQuery query, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return new ServiceResponse<UserProfileUpdateResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            var user = await userRepository.GetByIdAsync(userId.Value);

            if (user == null)
            {
                return new ServiceResponse<UserProfileUpdateResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.UserNotFound)
                };
            }

            var userProfile = new UserProfileUpdateResponseDTO
            {
                Name = user.UserDetail.Name,
                Surname = user.UserDetail.Surname,
                Email = user.Email,
                UniverstiyId = user.UserDetail.UniverstiyId,
                Gender = user.UserDetail.Gender,
                DepartmentId = user.UserDetail.DepartmentId,
                BirthDate = user.UserDetail.BirthDate,
                Graduaiton_Date = user.UserDetail.Graduaiton_Date,
                IsNotified = user.UserDetail.IsNotified,
                ProfileImageUrl = user.UserDetail.ProfileImageUrl,
                Password = null
            };
            return ServiceResponse<UserProfileUpdateResponseDTO>.Success(null, userProfile);
        }
    }
}
