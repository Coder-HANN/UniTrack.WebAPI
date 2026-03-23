using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Club;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetClubDetailQueryHandler : IRequestHandler<GetClubDetailQuery, ServiceResponse<GetClubDetailResponseDTO>>
    {
        private readonly IClubRepository clubRepository;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;

        public GetClubDetailQueryHandler(
            IClubRepository clubRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService)
        {
            this.clubRepository = clubRepository;
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<GetClubDetailResponseDTO>> Handle(GetClubDetailQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            var club = await clubRepository.GetClubDetailByIdAsync(request.ClubId, userId);

            if (club == null)
            {
                return ServiceResponse<GetClubDetailResponseDTO>.Fail(
                    await localizationService.Get(ValidationKeys.ClubNotFound));
            }

            var userClub = userId != null
                ? club.UserClubs.FirstOrDefault(uc => uc.UserId == userId)
                : null;

            var response = new GetClubDetailResponseDTO
            {
                Id = club.Id,
                Name = club.Name,
                Description = club.Description,
                LongDescription = club.LongDescription,
                LogoUrl = club.LogoUrl,
                CoverImageUrl = club.CoverImageUrl,
                InstagramLink = club.InstagramLink,
                TwitterLink = club.TwitterLink,
                WebsiteLink = club.WebsiteLink,
                LinkedlnLink = club.LinkedlnLink,
                ContactEmail = club.ContectEmail,
                President = club.President,
                PresidentMail = club.PresidentMail,
                Tag = (int)club.Tag,
                FollowerCount = club.UserClubs.Count(uc => uc.IsFollowing),
                EventCount = club.Events.Count,
                UniversityName = club.University?.Name,
                CityName = club.City?.Name,
                ClubCreatedDate = club.ClubCreatedDate ?? default(DateOnly),
                IsFollowed = userClub?.IsFollowing ?? false,
                IsNotificationOn = userClub?.IsNotification ?? false
            };

            return ServiceResponse<GetClubDetailResponseDTO>.Success(null, response);
        }
    }
}
