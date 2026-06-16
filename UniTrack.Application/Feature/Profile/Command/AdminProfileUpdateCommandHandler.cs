using MediatR;
using Microsoft.AspNetCore.Identity;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Profile.Command
{
    public class AdminProfileUpdateCommandHandler : IRequestHandler<AdminProfileUpdateCommand, ServiceResponse<AdminProfileUpdateResponseDTO>>
    {
        private readonly ICurrentUserServices _currentUserServices;
        private readonly IUserDetailRepository _userDetailRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IPasswordHasher<User> passwordHash;

        public AdminProfileUpdateCommandHandler(
            ICurrentUserServices currentUserServices,
            IUserDetailRepository userDetailRepository,
            IUserRepository userRepository,
            ILocalizationService localizationService,
            IPasswordHasher<User> passwordHasher)
        {
            _currentUserServices = currentUserServices;
            _userDetailRepository = userDetailRepository;
            _userRepository = userRepository;
            _localizationService = localizationService;
            passwordHash = passwordHasher;
        }

        public async Task<ServiceResponse<AdminProfileUpdateResponseDTO>> Handle(AdminProfileUpdateCommand request, CancellationToken cancellationToken)
        {
            var adminId = _currentUserServices.CurrentUser();
            if (adminId == null)
                return ServiceResponse<AdminProfileUpdateResponseDTO>.Fail(await _localizationService.Get(ValidationKeys.NotAuthorized));

            var role = _currentUserServices.Role();
            if (role != Role.SuperAdmin)
                return ServiceResponse<AdminProfileUpdateResponseDTO>.Fail(await _localizationService.Get(ValidationKeys.NotAuthorized));

            var user = await _userRepository.GetAsync(u => u.Id == adminId);
            var userDetail = await _userDetailRepository.GetAsync(ud => ud.UserId == adminId);

            if (user == null || userDetail == null)
                return ServiceResponse<AdminProfileUpdateResponseDTO>.Fail(await _localizationService.Get(ValidationKeys.UserNotFound));

            bool isUpdated = false;

            // Temel bilgiler
            if (!string.IsNullOrWhiteSpace(request.Name) && userDetail.Name != request.Name)
            {
                userDetail.Name = request.Name;
                isUpdated = true;
            }

            if (request.UniverstiyId.HasValue && userDetail.UniverstiyId != request.UniverstiyId.Value)
            {
                userDetail.UniverstiyId = request.UniverstiyId.Value;
                isUpdated = true;
            }

            // Email kontrolü
            if (!string.IsNullOrWhiteSpace(request.Email) && user.Email != request.Email)
            {
                var allUsers = await _userRepository.GetAllAsync();
                if (allUsers.Any(u => u.Id != adminId && u.Email == request.Email))
                    return ServiceResponse<AdminProfileUpdateResponseDTO>.Fail(await _localizationService.Get(ValidationKeys.UserEmailAlreadyExists));

                user.Email = request.Email;
                isUpdated = true;
            }

            // Password
            // 1. Kullanıcı mevcut şifresini (NowPassword) girmiş mi?
            // Password update flow (strict validation)
            var hasNowPassword = !string.IsNullOrWhiteSpace(request.NowPassword);
            var hasNewPassword = !string.IsNullOrWhiteSpace(request.Password);

            // New password sent without current password -> reject
            if (!hasNowPassword && hasNewPassword)
            {
                return ServiceResponse<AdminProfileUpdateResponseDTO>.Fail(
                    await _localizationService.Get(ValidationKeys.CurrentPasswordRequired));
            }

            // Current password sent without new password -> reject
            if (hasNowPassword && !hasNewPassword)
            {
                return ServiceResponse<AdminProfileUpdateResponseDTO>.Fail(
                    await _localizationService.Get(ValidationKeys.NewPasswordRequired));
            }

            // Both provided -> verify and update
            if (hasNowPassword && hasNewPassword)
            {
                var currentPasswordVerification =
                    passwordHash.VerifyHashedPassword(user, user.Password, request.NowPassword!);

                if (currentPasswordVerification == PasswordVerificationResult.Failed)
                {
                    return ServiceResponse<AdminProfileUpdateResponseDTO>.Fail(
                        await _localizationService.Get(ValidationKeys.CurrentPasswordIncorrect));
                }

                var isSameAsOld =
                    passwordHash.VerifyHashedPassword(user, user.Password, request.Password!);

                if (isSameAsOld == PasswordVerificationResult.Success)
                {
                    return ServiceResponse<AdminProfileUpdateResponseDTO>.Fail(
                        await _localizationService.Get(ValidationKeys.NewPasswordCannotBeSameAsOld));
                }

                user.Password = passwordHash.HashPassword(user, request.Password!);
                isUpdated = true;
            }

            // Fotoğraf alanları: güncelleme veya silme
            // Yeni — sadece geçerli URL veya null kabul ediyor
            if (request.ProfileImageUrl != null)
            {
                if (string.IsNullOrWhiteSpace(request.ProfileImageUrl))
                {
                    userDetail.ProfileImageUrl = null;
                    isUpdated = true;
                }
                else if (request.ProfileImageUrl.StartsWith("http://") ||
                         request.ProfileImageUrl.StartsWith("https://") ||
                         request.ProfileImageUrl.StartsWith("/"))
                {
                    userDetail.ProfileImageUrl = request.ProfileImageUrl;
                    isUpdated = true;
                }
            }

            // Notification
            if (request.IsNotified.HasValue && userDetail.IsNotified != request.IsNotified.Value)
            {
                userDetail.IsNotified = request.IsNotified.Value;
                isUpdated = true;
            }

            if (!isUpdated)
            {
                return ServiceResponse<AdminProfileUpdateResponseDTO>.Success(await _localizationService.Get(ValidationKeys.OperationSuccessful),
                    new AdminProfileUpdateResponseDTO
                    {
                        Name = userDetail.Name,
                        UniverstiyId = userDetail.UniverstiyId,
                        Email = user.Email,
                        ProfileImageUrl = userDetail.ProfileImageUrl,
                        IsNotified = userDetail.IsNotified,
                    }
                );
            }
            userDetail.UpdatedDate = DateTime.UtcNow;
            await _userDetailRepository.UpdateAsync(userDetail);
            await _userRepository.UpdateAsync(user);

            return new ServiceResponse<AdminProfileUpdateResponseDTO>
            {
                IsSuccess = true,
                Data = new AdminProfileUpdateResponseDTO
                {
                    Name = userDetail.Name,
                    UniverstiyId = userDetail.UniverstiyId,
                    Email = user.Email,
                    ProfileImageUrl = userDetail.ProfileImageUrl,
                    IsNotified = userDetail.IsNotified
                },
                Message = await _localizationService.Get(ValidationKeys.ProfileUpdatedSuccessfully)
            };
        }

    }
}