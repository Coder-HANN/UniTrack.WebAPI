using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.ClubTeam;

namespace UniTrack.Application.Feature.ClubTeam.Query
{
    public class ClubTeamQueryHandler : IRequestHandler<ClubTeamQuery, ServiceResponse<List<ClubTeamResponseDTO>>>
    {
        private readonly IClubTeamRepository clubTeamRepository;
        private readonly ILocalizationService localizationService;
        public ClubTeamQueryHandler(IClubTeamRepository clubTeamRepository, ILocalizationService localizationService)
        {
            this.clubTeamRepository = clubTeamRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<List<ClubTeamResponseDTO>>> Handle(ClubTeamQuery request, CancellationToken cancellationToken)
        {
            var clubTemas = await clubTeamRepository.GetClubTeamsByClubIdAsync(request.ClubId);

            if (clubTemas == null)
            {
                return new ServiceResponse<List<ClubTeamResponseDTO>>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.ClubTeamNotFound)
                };
            }

            var clubTeamDTOs = clubTemas.Select(ct => new ClubTeamResponseDTO
            {
                PersonName = ct.UserDetail.Name,
                PersonSurname = ct.UserDetail.Surname,
                Title = ct.Title

            }).ToList();

            return new ServiceResponse<List<ClubTeamResponseDTO>>
            {
                IsSuccess = true,
                Data = clubTeamDTOs,
                Message = null
            };
        }
    }
}
