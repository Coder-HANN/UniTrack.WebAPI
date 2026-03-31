using MediatR;
using Microsoft.AspNetCore.Identity;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Profile.Command
{
    public class ClubProfileUpdateCommandHandler : IRequestHandler<ClubProfileUpdateCommand, ServiceResponse<ClubProfileUpdateResponseDTO>>
    {
        private readonly ICurrentUserServices _currentUserServices;
        private readonly IClubRepository _clubRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IPasswordHasher<Domain.Entities.Club> _passwordHasher;

        public ClubProfileUpdateCommandHandler(
            ICurrentUserServices currentUserServices,
            IClubRepository clubRepository,
            IUserRepository userRepository,
            ILocalizationService localizationService,
            IPasswordHasher<Domain.Entities.Club> passwordHasher)
        {
            _currentUserServices = currentUserServices;
            _clubRepository = clubRepository;
            _userRepository = userRepository;
            _localizationService = localizationService;
            _passwordHasher = passwordHasher;
        }

        public async Task<ServiceResponse<ClubProfileUpdateResponseDTO>> Handle(ClubProfileUpdateCommand request, CancellationToken cancellationToken)
        {
            var clubId = _currentUserServices.CurrentClub();
            if (clubId == null)
            {
                return ServiceResponse<ClubProfileUpdateResponseDTO>.Fail(
                    await _localizationService.Get(ValidationKeys.NotAuthorized)
                );
            }

            var role = _currentUserServices.Role();
            if (role == null || role == Role.User)
            {
                return ServiceResponse<ClubProfileUpdateResponseDTO>.Fail(
                    await _localizationService.Get(ValidationKeys.NotAuthorized)
                );
            }

            var existingClub = await _clubRepository.GetAsync(c => c.Id == clubId);
            if (existingClub == null)
            {
                return ServiceResponse<ClubProfileUpdateResponseDTO>.Fail(
                    await _localizationService.Get(ValidationKeys.ClubNotFound)
                );
            }

            var allClubs = await _clubRepository.GetAllAsync();
            var allUsers = await _userRepository.GetAllAsync();

            bool isUpdated = false;

            // Temel alanlar
            if (!string.IsNullOrEmpty(request.Name) && existingClub.Name != request.Name)
            {
                existingClub.Name = request.Name;
                isUpdated = true;
            }
            if (!string.IsNullOrEmpty(request.Description) && existingClub.Description != request.Description)
            {
                existingClub.Description = request.Description;
                isUpdated = true;
            }
            if (!string.IsNullOrEmpty(request.LongDescription) && existingClub.LongDescription != request.LongDescription)
            {
                existingClub.LongDescription = request.LongDescription;
                isUpdated = true;
            }
            if (!string.IsNullOrEmpty(request.President) && existingClub.President != request.President)
            {
                existingClub.President = request.President;
                isUpdated = true;
            }
            if (!string.IsNullOrEmpty(request.PresidentMail) && existingClub.PresidentMail != request.PresidentMail)
            {
                // Mail çakışma kontrolü
                bool mailExists = allClubs.Any(c => c.Id != existingClub.Id &&
                                                   (c.PresidentMail == request.PresidentMail || c.ContectEmail == request.PresidentMail)) ||
                                  allUsers.Any(u => u.Email == request.PresidentMail);

                if (mailExists)
                {
                    return ServiceResponse<ClubProfileUpdateResponseDTO>.Fail(await _localizationService.Get(ValidationKeys.EmailAlreadyUsed)
                    );
                }

                existingClub.PresidentMail = request.PresidentMail;
                isUpdated = true;
            }
            if (!string.IsNullOrEmpty(request.ContectEmail) && existingClub.ContectEmail != request.ContectEmail)
            {
                // Mail çakışma kontrolü
                bool mailExists = allClubs.Any(c => c.Id != existingClub.Id &&
                                                   (c.ContectEmail == request.ContectEmail || c.PresidentMail == request.ContectEmail)) ||
                                  allUsers.Any(u => u.Email == request.ContectEmail);

                if (mailExists)
                {
                    return ServiceResponse<ClubProfileUpdateResponseDTO>.Fail(
                        await _localizationService.Get(ValidationKeys.EmailAlreadyUsed)
                    );
                }

                existingClub.ContectEmail = request.ContectEmail;
                isUpdated = true;
            }

            // Sosyal ve web linkleri
            // Sosyal ve web linkleri
            if (request.LinkedlnLink != null && existingClub.LinkedlnLink != request.LinkedlnLink)
            {
                existingClub.LinkedlnLink = string.IsNullOrEmpty(request.LinkedlnLink) ? null : request.LinkedlnLink;
                isUpdated = true;
            }
            if (request.InstagramLink != null && existingClub.InstagramLink != request.InstagramLink)
            {
                existingClub.InstagramLink = string.IsNullOrEmpty(request.InstagramLink) ? null : request.InstagramLink;
                isUpdated = true;
            }
            if (request.TwitterLink != null && existingClub.TwitterLink != request.TwitterLink)
            {
                existingClub.TwitterLink = string.IsNullOrEmpty(request.TwitterLink) ? null : request.TwitterLink;
                isUpdated = true;
            }
            if (request.WebsiteLink != null && existingClub.WebsiteLink != request.WebsiteLink)
            {
                existingClub.WebsiteLink = string.IsNullOrEmpty(request.WebsiteLink) ? null : request.WebsiteLink;
                isUpdated = true;
            }
            if (request.TikTokLink != null && existingClub.TikTokLink != request.TikTokLink)
            {
                existingClub.TikTokLink = string.IsNullOrEmpty(request.TikTokLink) ? null : request.TikTokLink;
                isUpdated = true;
            }
            // Tag
            if (request.Tag.HasValue && existingClub.Tag != request.Tag.Value)
            {
                existingClub.Tag = request.Tag.Value;
                isUpdated = true;
            }

            // Logo ve kapak: güncelleme veya silme
            if (request.LogoUrl != null && existingClub.LogoUrl != request.LogoUrl)
            {
                existingClub.LogoUrl = string.IsNullOrEmpty(request.LogoUrl) ? null : request.LogoUrl;
                isUpdated = true;
            }
            if (request.CoverImageUrl != null && existingClub.CoverImageUrl != request.CoverImageUrl)
            {
                existingClub.CoverImageUrl = string.IsNullOrEmpty(request.CoverImageUrl) ? null : request.CoverImageUrl;
                isUpdated = true;
            }
            // University & ClubCreatedDate
            if (request.UniversityId.HasValue && existingClub.UniversityId != request.UniversityId.Value)
            {
                existingClub.UniversityId = request.UniversityId.Value;
                isUpdated = true;
            }
            if (request.ClubCreatedDate.HasValue && existingClub.ClubCreatedDate != request.ClubCreatedDate.Value)
            {
                existingClub.ClubCreatedDate = request.ClubCreatedDate.Value;
                isUpdated = true;
            }

            if (!isUpdated)
            {
                return ServiceResponse<ClubProfileUpdateResponseDTO>.Success(await _localizationService.Get(ValidationKeys.OperationSuccessful),
                    null
                );
            }

            // Şifre değişikliği
            var hasNowPassword = !string.IsNullOrWhiteSpace(request.NowPassword);
            var hasNewPassword = !string.IsNullOrWhiteSpace(request.Password);

            if (!hasNowPassword && hasNewPassword)
                return ServiceResponse<ClubProfileUpdateResponseDTO>.Fail(
                    await _localizationService.Get(ValidationKeys.CurrentPasswordRequired));

            if (hasNowPassword && !hasNewPassword)
                return ServiceResponse<ClubProfileUpdateResponseDTO>.Fail(
                    await _localizationService.Get(ValidationKeys.NewPasswordRequired));

            if (hasNowPassword && hasNewPassword)
            {
                var verification = _passwordHasher.VerifyHashedPassword(existingClub, existingClub.Password, request.NowPassword!);
                if (verification == PasswordVerificationResult.Failed)
                    return ServiceResponse<ClubProfileUpdateResponseDTO>.Fail(
                        await _localizationService.Get(ValidationKeys.CurrentPasswordIncorrect));

                var isSame = _passwordHasher.VerifyHashedPassword(existingClub, existingClub.Password, request.Password!);
                if (isSame == PasswordVerificationResult.Success)
                    return ServiceResponse<ClubProfileUpdateResponseDTO>.Fail(
                        await _localizationService.Get(ValidationKeys.NewPasswordCannotBeSameAsOld));

                existingClub.Password = _passwordHasher.HashPassword(existingClub, request.Password!);
                isUpdated = true;
            }
            DateTime now = DateTime.UtcNow;
            existingClub.UpdatedDate = now;

            await _clubRepository.UpdateAsync(existingClub);

            var responseDto = new ClubProfileUpdateResponseDTO
            {
                Name = existingClub.Name,
                Description = existingClub.Description,
                LongDescription = existingClub.LongDescription,
                LinkedlnLink = existingClub.LinkedlnLink,
                InstagramLink = existingClub.InstagramLink,
                WebsiteLink = existingClub.WebsiteLink,
                TwitterLink = existingClub.TwitterLink,
                TikTokLink = existingClub.TikTokLink,
                Tag = existingClub.Tag,
                LogoUrl = existingClub.LogoUrl,
                CoverImageUrl = existingClub.CoverImageUrl,
                President = existingClub.President,
                PresidentMail = existingClub.PresidentMail,
                UniversityId = existingClub.UniversityId,
                ContectEmail = existingClub.ContectEmail
            };

            return ServiceResponse<ClubProfileUpdateResponseDTO>.Success(await _localizationService.Get(ValidationKeys.ProfileUpdatedSuccessfully, responseDto)
            );
        }
    }
}
