using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Transaction;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Opportunity.Command
{
    public class CreateOpportunityCommandHandler: IRequestHandler<CreateOpportunityCommand, ServiceResponse<string>>
    {
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly ICurrentUserServices _currentUserServices;
        private readonly ILocalizationService _localizationService;
        private readonly ITransactionService transaction;

        public CreateOpportunityCommandHandler(
            IOpportunityRepository opportunityRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService,
            ITransactionService transaction)
        {
            _opportunityRepository = opportunityRepository;
            _currentUserServices = currentUserServices;
            _localizationService = localizationService;
            this.transaction = transaction;
        }

        public async Task<ServiceResponse<string>> Handle(CreateOpportunityCommand request, CancellationToken cancellationToken)
        {
            var isAdmin = _currentUserServices.Role();

            if (isAdmin != Domain.Enums.Role.Admin)
                return ServiceResponse<string>.Fail(await _localizationService.Get(ValidationKeys.NotAuthorized));
            transaction.Begin();
            
            try { 
            var opportunity = new Domain.Entities.Opportunity
            {
                Id = Guid.NewGuid(),
                CompanyName = request.CompanyName,
                ImageUrl = request.ImageUrl,
                Title = request.Title,
                Description = request.Description,
                Link = request.Link,
                Category = request.Category,
                LastDate = request.LastDate,
                Code = request.Code
            };

            await _opportunityRepository.AddAsync(opportunity);

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                return ServiceResponse<string>.Fail(await _localizationService.Get(ValidationKeys.OperationFailed));
            }

            return ServiceResponse<string>.Success(await _localizationService.Get(ValidationKeys.OperationSuccessful));
        }
    }
}