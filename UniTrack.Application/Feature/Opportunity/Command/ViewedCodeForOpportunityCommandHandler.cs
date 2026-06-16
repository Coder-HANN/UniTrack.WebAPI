using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Opportunity;

namespace UniTrack.Application.Feature.Opportunity.Command
{
    public class ViewedCodeForOpportunityCommandHandler : IRequestHandler<ViewedCodeForOpportunityCommand, ServiceResponse<ViewedCodeForOpportunityResponseDTO>>
    {
        private readonly IOpportunityRepository opportunityRepository;
        private readonly ILocalizationService localizationService;
        private readonly ICurrentUserServices currentUserServices;

        public ViewedCodeForOpportunityCommandHandler(
            IOpportunityRepository opportunityRepository,
            ILocalizationService localizationService,
            ICurrentUserServices currentUserServices)
        {
            this.opportunityRepository = opportunityRepository;
            this.localizationService = localizationService;
            this.currentUserServices = currentUserServices;
        }

        public async Task<ServiceResponse<ViewedCodeForOpportunityResponseDTO>> Handle(ViewedCodeForOpportunityCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();

            if (!userId.HasValue)
            {
                return new ServiceResponse<ViewedCodeForOpportunityResponseDTO>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            var opportunity = await opportunityRepository.GetByIdAsync(request.OpportunityId);

            if (opportunity == null)
            {
                return new ServiceResponse<ViewedCodeForOpportunityResponseDTO>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.OpportunityNotFound)
                };
            }

            var opportunityUser = opportunity.OpportunityUsers
                .FirstOrDefault(ou => ou.UserId == userId.Value && ou.OpportunityId == request.OpportunityId);

            if (opportunityUser == null)
            {
                // OpportunityUser kaydı yoksa yeni oluştur
                opportunityUser = new Domain.Entities.OpportunityUser
                {
                    Id = Guid.NewGuid(),
                    UserId = userId.Value,
                    OpportunityId = opportunity.Id,
                    Viewed = true,
                    CreatedDate = DateTime.UtcNow
                };

                opportunity.OpportunityUsers.Add(opportunityUser);
            }
            else
            {
                opportunityUser.Viewed = true;
            }

            await opportunityRepository.UpdateAsync(opportunity);

            return new ServiceResponse<ViewedCodeForOpportunityResponseDTO>
            {
                IsSuccess = true,
                Data = new ViewedCodeForOpportunityResponseDTO
                {
                    Id = opportunity.Id,
                    Code = opportunity.Code
                },
                Message = await localizationService.Get(ValidationKeys.OperationSuccessful)
            };
        }
    }
}