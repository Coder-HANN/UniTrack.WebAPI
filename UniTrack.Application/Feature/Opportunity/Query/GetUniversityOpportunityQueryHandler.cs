using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Opportunity;

namespace UniTrack.Application.Feature.Opportunity.Query
{
    public class GetUniversityOpportunityQueryHandler : IRequestHandler<GetUniversityOpportunityQuery, ServiceResponse<IPagingExecutionResult<GetAllOpportunityResponseDTO>>>
    {
        private readonly IOpportunityRepository opportunityRepository;
        private readonly IBaseEntityRepository<Domain.Entities.Opportunity> baseEntityRepository;
        private readonly ILocalizationService localizationService;
        private readonly ICurrentUserServices currentUserServices;

        public GetUniversityOpportunityQueryHandler(
            IOpportunityRepository opportunityRepository,
            IBaseEntityRepository<Domain.Entities.Opportunity> baseEntityRepository,
            ILocalizationService localizationService,
            ICurrentUserServices currentUserServices)
        {
            this.opportunityRepository = opportunityRepository;
            this.baseEntityRepository = baseEntityRepository;
            this.localizationService = localizationService;
            this.currentUserServices = currentUserServices;
        }

        public async Task<ServiceResponse<IPagingExecutionResult<GetAllOpportunityResponseDTO>>> Handle(GetUniversityOpportunityQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            var currentUniversityId = currentUserServices.UniversityId();

            if (currentUniversityId == null)
                return ServiceResponse<IPagingExecutionResult<GetAllOpportunityResponseDTO>>.Fail(
                    await localizationService.Get(ValidationKeys.NotAuthorized));

            var opportunities = await opportunityRepository.GetOpportunitiesByUniversityAsync(currentUniversityId.Value);

            if (opportunities == null || opportunities.Count == 0)
            {
                return new ServiceResponse<IPagingExecutionResult<GetAllOpportunityResponseDTO>>
                {
                    IsSuccess = true,
                    Data = await baseEntityRepository.GetPagedResult(
                        Enumerable.Empty<GetAllOpportunityResponseDTO>(),
                        pageSize: request.PageSize,
                        pageIndex: request.Page,
                        ordering: null,
                        cancellationToken: cancellationToken),
                    Message = await localizationService.Get(ValidationKeys.OpportunityNotFound)
                };
            }

            var responses = opportunities.Select(o =>
            {
                var opportunityUser = userId.HasValue
                    ? o.OpportunityUsers.FirstOrDefault(ou => ou.UserId == userId.Value)
                    : null;

                var hasViewed = opportunityUser?.Viewed ?? false;

                return new GetAllOpportunityResponseDTO
                {
                    Id = o.Id,
                    CompanyName = o.CompanyName,
                    ImageUrl = o.ImageUrl,
                    Title = o.Title,
                    Description = o.Description,
                    Link = o.Link,
                    Category = o.Category.ToString(),
                    LastDate = o.LastDate,
                    Code = hasViewed ? o.Code : null,
                    Viewed = hasViewed
                };
            })
            .OrderByDescending(o => o.LastDate)
            .ToList();

            var result = await baseEntityRepository.GetPagedResult(
                responses,
                pageSize: request.PageSize,
                pageIndex: request.Page,
                ordering: null,
                cancellationToken: cancellationToken
            );

            return new ServiceResponse<IPagingExecutionResult<GetAllOpportunityResponseDTO>>
            {
                IsSuccess = true,
                Data = result,
                Message = await localizationService.Get(ValidationKeys.OperationSuccessful)
            };
        }
    }
}