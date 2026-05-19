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
            var userId = _currentUserServices.Role();

            if (userId != Domain.Enums.Role.Admin)
                return ServiceResponse<string>.Fail(await _localizationService.Get(ValidationKeys.NotAuthorized));

            var opportunity = await _opportunityRepository.GetByIdAsync(request.Id);

            if (opportunity == null)
                return ServiceResponse<string>.Fail(await _localizationService.Get(ValidationKeys.OpportunityNotFound));

            opportunity.IsDeleted = true;

            await _opportunityRepository.UpdateAsync(opportunity);

            return ServiceResponse<string>.Success(await _localizationService.Get(ValidationKeys.OperationSuccessful));
        }
    }
}