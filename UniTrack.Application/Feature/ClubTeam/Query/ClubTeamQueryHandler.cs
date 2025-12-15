using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.ClubTeam;

namespace UniTrack.Application.Feature.ClubTeam.Query
{
    public class ClubTeamQueryHandler : IRequestHandler<ClubTeamQuery, ServiceResponse<List<ClubTeamResponseDTO>>>
    {
        private readonly IClubTeamRepository clubTeamRepository;
        public ClubTeamQueryHandler(IClubTeamRepository clubTeamRepository)
        {
            this.clubTeamRepository = clubTeamRepository;
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
                    Message = "Yönetim kurulu bulunamadı."
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
