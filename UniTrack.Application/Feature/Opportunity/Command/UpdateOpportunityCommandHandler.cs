using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Transaction;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Opportunity.Command
{
    public class UpdateOpportunityCommandHandler : IRequestHandler<UpdateOpportunityCommand, ServiceResponse<string>>
    {
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly ICurrentUserServices _currentUserServices;
        private readonly ILocalizationService _localizationService;
        private readonly ITransactionService _transaction;

        public UpdateOpportunityCommandHandler(
            IOpportunityRepository opportunityRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService,
            ITransactionService transaction)
        {
            _opportunityRepository = opportunityRepository;
            _currentUserServices = currentUserServices;
            _localizationService = localizationService;
            _transaction = transaction;
        }

        public async Task<ServiceResponse<string>> Handle(UpdateOpportunityCommand request, CancellationToken cancellationToken)
        {
            var role = _currentUserServices.Role();
            var currentUniversityId = _currentUserServices.UniversityId();

            if (role != Domain.Enums.Role.SuperAdmin && role != Domain.Enums.Role.Admin)
                return ServiceResponse<string>.Fail(await _localizationService.Get(ValidationKeys.NotAuthorized));

            var opportunity = await _opportunityRepository.GetByIdAsync(request.Id);
            if (opportunity is null)
                return ServiceResponse<string>.Fail(await _localizationService.Get(ValidationKeys.OpportunityNotFound));

            // Admin sadece kendi üniversitesine ait fırsatı güncelleyebilir
            if (role == Domain.Enums.Role.Admin)
            {
                if (currentUniversityId == null)
                    return ServiceResponse<string>.Fail(await _localizationService.Get(ValidationKeys.NotAuthorized));

                var belongsToUniversity = opportunity.OpportunityUniversities
                    .Any(x => x.UniversityId == currentUniversityId.Value);

                if (!belongsToUniversity)
                    return ServiceResponse<string>.Fail(await _localizationService.Get(ValidationKeys.NotAuthorized));
            }

            _transaction.Begin();
            try
            {
                opportunity.CompanyName = request.CompanyName;
                opportunity.ImageUrl = request.ImageUrl;
                opportunity.Title = request.Title;
                opportunity.Description = request.Description;
                opportunity.Link = request.Link;
                opportunity.Category = request.Category;
                opportunity.LastDate = request.LastDate;
                opportunity.Code = request.Code;

                await _opportunityRepository.UpdateAsync(opportunity);
                _transaction.Commit();
            }
            catch (Exception)
            {
                _transaction.Rollback();
                return ServiceResponse<string>.Fail(await _localizationService.Get(ValidationKeys.OperationFailed));
            }

            return ServiceResponse<string>.Success(await _localizationService.Get(ValidationKeys.OperationSuccessful));
        }
    }
}