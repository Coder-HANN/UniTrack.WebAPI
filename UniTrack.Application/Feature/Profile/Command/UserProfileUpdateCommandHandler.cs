using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Profile.Command
{
    public class UserProfileUpdateCommandHandler : IRequestHandler<UserProfileUpdateCommand, ServiceResponse<UserProfileUpdateResponseDTO>>
    {
        private readonly ICurrentUserServices _currentUserServices;
        private readonly IUserDetailRepository _userDetailRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILocalizationService _localizationService;

        public UserProfileUpdateCommandHandler(
            ICurrentUserServices currentUserServices,
            IUserDetailRepository userDetailRepository,
            IUserRepository userRepository,
            ILocalizationService localizationService)
        {
            _currentUserServices = currentUserServices;
            _userDetailRepository = userDetailRepository;
            _userRepository = userRepository;
            _localizationService = localizationService;
        }

        public async Task<ServiceResponse<UserProfileUpdateResponseDTO>> Handle(UserProfileUpdateCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserServices.CurrentUser();
            if (userId == null)
                return ServiceResponse<UserProfileUpdateResponseDTO>.Fail(await _localizationService.Get(ValidationKeys.NotAuthorized));

            var role = _currentUserServices.Role();
            if (role == null || role == Role.Club)
                return ServiceResponse<UserProfileUpdateResponseDTO>.Fail(await _localizationService.Get(ValidationKeys.NotAuthorized));

            var user = await _userRepository.GetAsync(u => u.Id == userId);
            var userDetail = await _userDetailRepository.GetAsync(ud => ud.UserId == userId);

            if (user == null || userDetail == null)
                return ServiceResponse<UserProfileUpdateResponseDTO>.Fail(await _localizationService.Get(ValidationKeys.UserNotFound));

            bool isUpdated = false;

            // Temel bilgiler
            if (!string.IsNullOrWhiteSpace(request.Name) && userDetail.Name != request.Name)
            {
                userDetail.Name = request.Name;
                isUpdated = true;
            }
            if (!string.IsNullOrWhiteSpace(request.Surname) && userDetail.Surname != request.Surname)
            {
                userDetail.Surname = request.Surname;
                isUpdated = true;
            }

            // Nullable alanlar
            if (request.DepartmentId.HasValue && userDetail.DepartmentId != request.DepartmentId.Value)
            {
                userDetail.DepartmentId = request.DepartmentId.Value;
                isUpdated = true;
            }
            if (request.UniverstiyId.HasValue && userDetail.UniverstiyId != request.UniverstiyId.Value)
            {
                userDetail.UniverstiyId = request.UniverstiyId.Value;
                isUpdated = true;
            }
            if (request.BirthDate.HasValue && userDetail.BirthDate != request.BirthDate.Value)
            {
                userDetail.BirthDate = request.BirthDate.Value;
                isUpdated = true;
            }
            if (request.Gender.HasValue && userDetail.Gender != request.Gender.Value)
            {
                userDetail.Gender = request.Gender.Value;
                isUpdated = true;
            }
            if (request.Graduaiton_Date.HasValue && userDetail.Graduaiton_Date != request.Graduaiton_Date.Value)
            {
                userDetail.Graduaiton_Date = request.Graduaiton_Date.Value;
                isUpdated = true;
            }
            if (request.PhoneNumber.HasValue && userDetail.PhoneNumber != request.PhoneNumber.Value)
            {
                userDetail.PhoneNumber = request.PhoneNumber.Value;
                isUpdated = true;
            }

            // Email kontrolü
            if (!string.IsNullOrWhiteSpace(request.Email) && user.Email != request.Email)
            {
                var allUsers = await _userRepository.GetAllAsync();
                if (allUsers.Any(u => u.Id != userId && u.Email == request.Email))
                    return ServiceResponse<UserProfileUpdateResponseDTO>.Fail(await _localizationService.Get(ValidationKeys.UserEmailAlreadyExists));

                user.Email = request.Email;
                isUpdated = true;
            }

            // Password
            if (!string.IsNullOrWhiteSpace(request.Password) && user.Password != request.Password)
            {
                user.Password = request.Password;
                isUpdated = true;
            }

            // Fotoğraf alanları: güncelleme veya silme
            if (request.ProfileImageUrl != null)
            {
                userDetail.ProfileImageUrl = string.IsNullOrWhiteSpace(request.ProfileImageUrl) ? null : request.ProfileImageUrl;
                isUpdated = true;
            }

            // Notification
            if (request.IsNotified.HasValue && userDetail.IsNotified != request.IsNotified.Value)
            {
                userDetail.IsNotified = request.IsNotified.Value;
                isUpdated = true;
            }

            if (!isUpdated)
            {
                return ServiceResponse<UserProfileUpdateResponseDTO>.Success(await _localizationService.Get(ValidationKeys.OperationSuccessful),
                    new UserProfileUpdateResponseDTO
                    {
                        Name = userDetail.Name,
                        Surname = userDetail.Surname,
                        DepartmentId = userDetail.DepartmentId,
                        UniverstiyId = userDetail.UniverstiyId,
                        Email = user.Email,
                        BirthDate = userDetail.BirthDate,
                        Gender = userDetail.Gender,
                        ProfileImageUrl = userDetail.ProfileImageUrl,
                        IsNotified = userDetail.IsNotified
                    }
                    
                );
            }

            await _userDetailRepository.UpdateAsync(userDetail);
            await _userRepository.UpdateAsync(user);

            return new ServiceResponse<UserProfileUpdateResponseDTO>
            {
                IsSuccess = true,
                Data = new UserProfileUpdateResponseDTO
                {
                    Name = userDetail.Name,
                    Surname = userDetail.Surname,
                    DepartmentId = userDetail.DepartmentId,
                    UniverstiyId = userDetail.UniverstiyId,
                    Email = user.Email,
                    BirthDate = userDetail.BirthDate,
                    Gender = userDetail.Gender,
                    ProfileImageUrl = userDetail.ProfileImageUrl,
                    IsNotified = userDetail.IsNotified
                },
                Message = await _localizationService.Get(ValidationKeys.ProfileUpdatedSuccessfully)
            };
        }      
            
    }
}