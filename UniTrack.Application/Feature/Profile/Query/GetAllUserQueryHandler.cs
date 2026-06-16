using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Profile.Query
{
    public class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, ServiceResponse<List<GetAllUserQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUserRepository userRepository;
        private readonly ILocalizationService localizationService;

        public GetAllUserQueryHandler(
            ICurrentUserServices currentUserServices,
            IUserRepository userRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.userRepository = userRepository;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<List<GetAllUserQueryResponseDTO>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return new ServiceResponse<List<GetAllUserQueryResponseDTO>> {
                    
                        IsSuccess = false,
                        Data = null,
                        Message = await localizationService.Get(ValidationKeys.NotAuthorized)

                };
            }

            var role = currentUserServices.Role();

            if (role == null || role == Role.Club || role == Role.User)
            {
                 return new ServiceResponse<List<GetAllUserQueryResponseDTO>>
                 { 
                    
                         IsSuccess = false,
                         Data = null,
                         Message = await localizationService.Get(ValidationKeys.NotAuthorized)

                 };
            }

            var users = await userRepository.GetAllAsync();
            if (users == null || users.Count == 0)
            {
                return new ServiceResponse<List<GetAllUserQueryResponseDTO>> {
                        IsSuccess = false,
                        Data = null,
                        Message = "Kullanıcı bulunamadı."
                };
            }

            var responses = users.Select(u => new GetAllUserQueryResponseDTO
            {
                    UserId = u.Id,
                    Name = u.UserDetail.Name,
                    Surname = u.UserDetail.Surname,
                    Email = u.Email,
                    UniversityId = u.UserDetail.UniverstiyId.Value,
                    DepartmentId = u.UserDetail.DepartmentId.Value,
                
            }).ToList();

            return new ServiceResponse<List<GetAllUserQueryResponseDTO>>
            {
                IsSuccess = true,
                Data = responses,
                Message = "Toplam kullanıcı sayısı: " + responses.Count.ToString()
            };
        }
    }
}
