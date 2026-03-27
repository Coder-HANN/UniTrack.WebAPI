using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Profile;

namespace UniTrack.Application.Feature.Profile.Query
{
    public class GetClubSettingsQueryHandler : IRequestHandler<GetClubSettingsQuery, ServiceResponse<ClubSettingsResponseDTO>>
    {
        private readonly IClubRepository clubRepository;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;

        public GetClubSettingsQueryHandler(
            IClubRepository clubRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService)
        {
            this.clubRepository = clubRepository;
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<ClubSettingsResponseDTO>> Handle(GetClubSettingsQuery request,CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();
            if (clubId == null)
                return ServiceResponse<ClubSettingsResponseDTO>.Fail(
                    await localizationService.Get(ValidationKeys.NotAuthorized));

            var club = await clubRepository.GetAsync(c => c.Id == clubId);
            if (club == null)
                return ServiceResponse<ClubSettingsResponseDTO>.Fail(
                    await localizationService.Get(ValidationKeys.ClubNotFound));

            return ServiceResponse<ClubSettingsResponseDTO>.Success(null, new ClubSettingsResponseDTO
            {
                Name = club.Name,
                UniversityId = club.UniversityId,
                President = club.President,
                PresidentMail = club.PresidentMail,
                ContectEmail = club.ContectEmail,
                Description = club.Description,
                LongDescription = club.LongDescription,
                InstagramLink = club.InstagramLink,
                TwitterLink = club.TwitterLink,
                WebsiteLink = club.WebsiteLink,
                LinkedlnLink = club.LinkedlnLink,
                TikTokLink = club.TikTokLink,
                LogoUrl = club.LogoUrl,
                CoverImageUrl = club.CoverImageUrl,
                Tag = club.Tag,
                ClubCreatedDate = club.ClubCreatedDate
            });
        }
    }
}