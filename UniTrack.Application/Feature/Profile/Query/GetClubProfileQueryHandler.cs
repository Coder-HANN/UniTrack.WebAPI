using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Profile;

namespace UniTrack.Application.Feature.Profile.Query
{
    public class GetClubProfileQueryHandler : IRequestHandler<GetClubProfileQuery, ServiceResponse<GetClubProfileResponseDTO>>
    {
        private readonly ICurrentUserServices _currentUserServices;
        private readonly IClubRepository _clubRepository;
        private readonly ILocalizationService _localizationService;

        public GetClubProfileQueryHandler(
            ICurrentUserServices currentUserServices,
            IClubRepository clubRepository,
            ILocalizationService localizationService)
        {
            _currentUserServices = currentUserServices;
            _clubRepository = clubRepository;
            _localizationService = localizationService;
        }

        public async Task<ServiceResponse<GetClubProfileResponseDTO>> Handle(GetClubProfileQuery request,CancellationToken cancellationToken)
        {
            var clubId = _currentUserServices.CurrentClub();
            if (clubId == null)
                return ServiceResponse<GetClubProfileResponseDTO>.Fail(await _localizationService.Get(ValidationKeys.NotAuthorized));

            var club = await _clubRepository.GetAsync(c => c.Id == clubId);
            if (club == null)
                return ServiceResponse<GetClubProfileResponseDTO>.Fail(await _localizationService.Get(ValidationKeys.ClubNotFound));

            return ServiceResponse<GetClubProfileResponseDTO>.Success(null,new GetClubProfileResponseDTO
            {
                    Name = club.Name ?? string.Empty,
                    ContactMail = club.ContectEmail ?? string.Empty,
                    LogoUrl = club.LogoUrl ?? string.Empty,
            });
        }
    }
}
