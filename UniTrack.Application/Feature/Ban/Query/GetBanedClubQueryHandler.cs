using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization; // Eklendi
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants; // Eklendi
using UniTrack.Application.DTOs.Ban;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Ban.Query
{
    public class GetBanedClubQueryHandler : IRequestHandler<GetBanedClubQuery, ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IBanRepository banRepository;
        private readonly ILocalizationService localizationService; // Eklendi

        public GetBanedClubQueryHandler(
            ICurrentUserServices currentUserServices,
            IBanRepository banRepository,
            ILocalizationService localizationService) // Eklendi
        {
            this.currentUserServices = currentUserServices;
            this.banRepository = banRepository;
            this.localizationService = localizationService; // Eklendi
        }

        public async Task<ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>>> Handle(GetBanedClubQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return new ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized) // Güncellendi
                };
            }
            var role = currentUserServices.Role();
            if (role == null || role == Role.Club || role == Role.User)
            {
                return new ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized) // Güncellendi
                };
            }

            // Testlerin kırılmaması için buradaki parametreye dikkat!
            var list = await banRepository.GetBannedClubInUniversityAsync(currentUserServices.UniversityId());
            if (list == null)
            {
                return new ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.OperationFailed) // Güncellendi (ValidationKeys içinde neyse)
                };
            }

            var responses = list.Select(b => new GetBanedClubOrUserQueryResponseDTO
            {
                Id = b.Id,
                Role = b.Club.Role,
                ClubId = b.ClubId,
                Name = b.Club.Name,
                Description = b.Description,
                CreatedDate = b.CreatedDate,
                LastDate = b.LastDate
            }).ToList();

            return new ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>>
            {
                IsSuccess = true,
                Data = responses,
                Message = await localizationService.Get(ValidationKeys.OperationSuccessful)
            };
        }
    }
}