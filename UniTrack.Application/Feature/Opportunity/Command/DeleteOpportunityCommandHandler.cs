using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Opportunity.Command
{
    public class DeleteOpportunityCommandHandler: IRequestHandler<DeleteOpportunityCommand, ServiceResponse<string>>
    {
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly ICurrentUserServices _currentUserServices;
        private readonly ILocalizationService _localizationService;

        public DeleteOpportunityCommandHandler(
            IOpportunityRepository opportunityRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService)
        {
            _opportunityRepository = opportunityRepository;
            _currentUserServices = currentUserServices;
            _localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(DeleteOpportunityCommand request, CancellationToken cancellationToken)
        {
            var role = _currentUserServices.Role();
            var currentUniversityId = _currentUserServices.UniversityId();

            if (role != Domain.Enums.Role.SuperAdmin && role != Domain.Enums.Role.Admin)
                return ServiceResponse<string>.Fail(await _localizationService.Get(ValidationKeys.NotAuthorized));

            var opportunity = await _opportunityRepository.GetByIdAsync(request.Id);

            if (opportunity == null)
                return ServiceResponse<string>.Fail(await _localizationService.Get(ValidationKeys.OpportunityNotFound));

            // Admin sadece kendi üniversitesine ait fırsatı silebilir
            if (role == Domain.Enums.Role.Admin)
            {
                if (currentUniversityId == null)
                    return ServiceResponse<string>.Fail(await _localizationService.Get(ValidationKeys.NotAuthorized));

                var belongsToUniversity = opportunity.OpportunityUniversities
                    .Any(x => x.UniversityId == currentUniversityId.Value);

                if (!belongsToUniversity)
                    return ServiceResponse<string>.Fail(await _localizationService.Get(ValidationKeys.NotAuthorized));
            }

            opportunity.IsDeleted = true;
            await _opportunityRepository.UpdateAsync(opportunity);

            return ServiceResponse<string>.Success(await _localizationService.Get(ValidationKeys.OperationSuccessful));
        }
    }
}