using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.University.Command
{
    public class CreateUniversityCommandHandler : IRequestHandler<CreateUniversityCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUniversityRepository universityRepository;
        private readonly ILocalizationService localizationService;
        public CreateUniversityCommandHandler(
            ICurrentUserServices currentUserServices,
            IUniversityRepository universityRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.universityRepository = universityRepository;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<string>> Handle(CreateUniversityCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();

            if (userId == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            var role = currentUserServices.Role();

            if (role == null || role == Role.User || role == Role.Club)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            await universityRepository.AddAsync(new Domain.Entities.University
            {
                Name = request.Name,
                CityId = request.CityId
            });

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Data = null,
                Message = await localizationService.Get(ValidationKeys.UniversityCreatedSuccesses)
            };
        }
    }
}