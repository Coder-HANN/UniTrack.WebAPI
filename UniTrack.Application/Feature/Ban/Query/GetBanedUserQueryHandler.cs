using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Ban;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Ban.Query
{
    public class GetBanedUserQueryHandler : IRequestHandler<GetBanedUserQuery, ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IBanRepository banRepository;
        private readonly ILocalizationService localizationService;

        public GetBanedUserQueryHandler(ICurrentUserServices currentUserServices, IBanRepository banRepository,ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.banRepository = banRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>>> Handle(GetBanedUserQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            var role = currentUserServices.Role();

            if (role != Role.Admin)
            {
                return new ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>> {
                 
                        IsSuccess = false,
                        Data = null,
                        Message = await localizationService.Get(ValidationKeys.NotAuthorized),

                };
            }

            var list = await banRepository.GetBannedUserInUniversityAsync(currentUserServices.UniversityId());
            if (list == null)
            {
                return new ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>> {
                    
                        IsSuccess = false,
                        Data = null,
                        Message = "Liste boş"
                    
                };
            }

            var responses = list.Select(b => new GetBanedClubOrUserQueryResponseDTO
            {
                
                    Id = b.Id,
                    Role = b.User.Role,
                    UserId = b.UserId,
                    Name = b.User.UserDetail.Name,
                    CreatedDate = b.CreatedDate,
                    LastDate = b.LastDate,
                    Description = b.Description
                
            }).ToList();

            return new ServiceResponse<List<GetBanedClubOrUserQueryResponseDTO>>
            {
                
                    IsSuccess = true,
                    Data = responses,
                    Message = await localizationService.Get(ValidationKeys.OperationSuccessful),

            };
        }
    }
}

