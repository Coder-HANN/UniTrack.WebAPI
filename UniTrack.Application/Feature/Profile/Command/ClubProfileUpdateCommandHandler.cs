using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Profile.Command
{
    public class ClubProfileUpdateCommandHandler : IRequestHandler<ClubProfileUpdateCommand, ServiceResponse<ClubProfileUpdateResponseDTO>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IClubRepository clubRepository;
        private readonly IUserRepository userRepository;
        public ClubProfileUpdateCommandHandler(
            ICurrentUserServices currentUserServices,
            IClubRepository clubRepository,
            IUserRepository userRepository)
        {
            this.currentUserServices = currentUserServices;
            this.clubRepository = clubRepository;
            this.userRepository = userRepository;
        }
        public async Task<ServiceResponse<ClubProfileUpdateResponseDTO>> Handle(ClubProfileUpdateCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentClub();
            if (userId == null)
            {
                return new ServiceResponse<ClubProfileUpdateResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Unauthorized"
                };
            }
            var role = currentUserServices.Role();
            if (role == null || role == Role.User)
            {
                return new ServiceResponse<ClubProfileUpdateResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Yetkisiz kullanıcı"
                };
            }

            var existingClub = await clubRepository.GetAsync(c => c.Id == userId);
            var allMail = await clubRepository.GetAllAsync();
           
            if (existingClub == null)
            {
                return new ServiceResponse<ClubProfileUpdateResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Club not found"
                };
            }

            bool isUpdated = false;

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
            if(!string.IsNullOrEmpty(request.LinkedlnLink) &&( existingClub.LinkedlnLink != request.LinkedlnLink))
            {
                existingClub.LinkedlnLink = request.LinkedlnLink;
                isUpdated = true;
            }
            if(!string.IsNullOrEmpty(request.InstagramLink) && (existingClub.InstagramLink != request.InstagramLink))
            {
                existingClub.InstagramLink = request.InstagramLink;
                isUpdated = true;
            }
            if(!string.IsNullOrEmpty(request.WebsiteLink) && (existingClub.WebsiteLink != request.WebsiteLink))
            {
                existingClub.WebsiteLink = request.WebsiteLink;
                isUpdated = true;
            }
            if(!string.IsNullOrEmpty(request.TwitterLink) && (existingClub.TwitterLink != request.TwitterLink))
            {
                existingClub.TwitterLink = request.TwitterLink;
                isUpdated = true;
            }
            if (!request.Tag.HasValue && request.Tag.Value != existingClub.Tag)
            {
                existingClub.Tag = request.Tag.Value;
                isUpdated = true;
            }
            if (!request.Logo.HasValue && request.Logo.Value != existingClub.Logo)
            {
                existingClub.Logo = request.Logo;
                isUpdated = true;
            }
            if(!request.CoverImage.HasValue && request.CoverImage.Value != existingClub.CoverImage)
            {
                existingClub.CoverImage = request.CoverImage;
                isUpdated = true;
            }
            if (!string.IsNullOrEmpty(request.President) && existingClub.President != request.President)
            {
                existingClub.President = request.President;
                isUpdated = true;
            }
            if (!string.IsNullOrEmpty(request.PresidentMail) && existingClub.PresidentMail != request.PresidentMail)
            {
                existingClub.PresidentMail = request.PresidentMail;
                isUpdated = true;
            }

            if(request.PresidentMail == existingClub.PresidentMail)
            if (!request.UniversityId.HasValue && request.UniversityId.Value != existingClub.UniversityId)
            {
                existingClub.UniversityId = request.UniversityId.Value;
                isUpdated = true;
            }
            if (!string.IsNullOrEmpty(request.ContectEmail) && existingClub.ContectEmail != request.ContectEmail)
            {
                existingClub.ContectEmail = request.ContectEmail;
                isUpdated = true;
            }
            if(!request.ClubCreatedDate.HasValue && request.ClubCreatedDate != existingClub.ClubCreatedDate)
            {
                existingClub.ClubCreatedDate = request.ClubCreatedDate.Value;
                isUpdated = true;
            }
            if (!isUpdated)
            {
                var response = new ClubProfileUpdateResponseDTO
                {
                        Name = existingClub.Name,
                        Description = existingClub.Description,
                        LongDescription = existingClub.LongDescription,
                        LinkedlnLink = existingClub.LinkedlnLink,
                        InstagramLink = existingClub.InstagramLink,
                        WebsiteLink = existingClub.WebsiteLink,
                        TwitterLink = existingClub.TwitterLink,
                        Tag = existingClub.Tag,
                        Logo = existingClub.Logo,
                        CoverImage = existingClub.CoverImage,
                        President = existingClub.President,
                        PresidentMail = existingClub.PresidentMail,
                        UniversityId = existingClub.UniversityId,
                        ContectEmail = existingClub.ContectEmail   
                };
            }

            var mailExists = allMail.Any(c => c.PresidentMail == request.PresidentMail || c.ContectEmail == request.ContectEmail);
            var ue = await userRepository.GetAllAsync();
            var user = ue.Any(u => u.Email == request.ContectEmail || u.Email == request.PresidentMail);

            if (user || mailExists)
            {
                return new ServiceResponse<ClubProfileUpdateResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Bu e-posta zaten kullanılıyor."
                };
            }

            await clubRepository.UpdateAsync(existingClub);

            return new ServiceResponse<ClubProfileUpdateResponseDTO>
            {
                IsSuccess = true,
                Data = null,
                Message = "İşlem başarılı"
            };
        }
    }
}
