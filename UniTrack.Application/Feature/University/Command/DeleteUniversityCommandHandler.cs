using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.University.Command
{
    public class DeleteUniversityCommandHandler : IRequestHandler<DeleteUniversityCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUniversityRepository universityRepository;
        private readonly ILocalizationService localizationService;
        public DeleteUniversityCommandHandler(
            ICurrentUserServices currentUserServices,
            IUniversityRepository universityRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.universityRepository = universityRepository;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<string>> Handle(DeleteUniversityCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }
            var role = currentUserServices.Role();
            if (role != Role.Admin)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }
            var existingUniversity = await universityRepository.GetByIdAsync(request.Id);
            if (existingUniversity == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.UniversityNotFound)
                };
            }
            await universityRepository.DeleteAsync(existingUniversity);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = await localizationService.Get(ValidationKeys.UniversityCreatedSuccesses)
            };
        }
    }
}
