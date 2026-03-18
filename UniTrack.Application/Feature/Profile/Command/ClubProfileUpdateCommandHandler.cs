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
    public class ClubProfileUpdateCommandHandler : IRequestHandler<ClubProfileUpdateCommand, ServiceResponse<ClubProfileUpdateResponseDTO>>
    {
        private readonly ICurrentUserServices _currentUserServices;
        private readonly IClubRepository _clubRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILocalizationService _localizationService;

        public ClubProfileUpdateCommandHandler(
            ICurrentUserServices currentUserServices,
            IClubRepository clubRepository,
            IUserRepository userRepository,
            ILocalizationService localizationService)
        {
            _currentUserServices = currentUserServices;
            _clubRepository = clubRepository;
            _userRepository = userRepository;
            _localizationService = localizationService;
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
            if (!string.IsNullOrEmpty(request.LinkedlnLink) && existingClub.LinkedlnLink != request.LinkedlnLink)
            {
                existingClub.LinkedlnLink = request.LinkedlnLink;
                isUpdated = true;
            }
            if (!string.IsNullOrEmpty(request.InstagramLink) && existingClub.InstagramLink != request.InstagramLink)
            {
                existingClub.InstagramLink = request.InstagramLink;
                isUpdated = true;
            }
            if (!string.IsNullOrEmpty(request.TwitterLink) && existingClub.TwitterLink != request.TwitterLink)
            {
                existingClub.TwitterLink = request.TwitterLink;
                isUpdated = true;
            }
            if (!string.IsNullOrEmpty(request.WebsiteLink) && existingClub.WebsiteLink != request.WebsiteLink)
            {
                existingClub.WebsiteLink = request.WebsiteLink;
                isUpdated = true;
            }

            // Tag
            if (request.Tag.HasValue && existingClub.Tag != request.Tag.Value)
            {
                existingClub.Tag = request.Tag.Value;
                isUpdated = true;
            }

           // Logo ve kapak: güncelleme veya silme
            if (request.LogoUrl != null)
            {
                existingClub.LogoUrl = string.IsNullOrWhiteSpace(request.LogoUrl) ? null : request.LogoUrl;
                isUpdated = true;
            }
            if (request.CoverImageUrl != null)
            {
                existingClub.CoverImageUrl = string.IsNullOrWhiteSpace(request.CoverImageUrl) ? null : request.CoverImageUrl;
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
