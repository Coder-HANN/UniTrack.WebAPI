using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Report;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Report.Query
{
    public class GetAllReportQueryCommandHandler : IRequestHandler<GetAllReportQueryCommand, ServiceResponse<IPagingExecutionResult<GetAllReportResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;
        private readonly IReportRepository reportRepository;
        private readonly IBaseEntityRepository<Domain.Entities.Report> baseEntityRepository;

        public GetAllReportQueryCommandHandler(
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService,
            IReportRepository reportRepository,
            IBaseEntityRepository<Domain.Entities.Report> baseEntityRepository)
        {
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
            this.reportRepository = reportRepository;
            this.baseEntityRepository = baseEntityRepository;
        }

        public async Task<ServiceResponse<IPagingExecutionResult<GetAllReportResponseDTO>>> Handle(GetAllReportQueryCommand request,CancellationToken cancellationToken)
        {
            var adminId = currentUserServices.CurrentUser();

            if (adminId == null || currentUserServices.Role() != Role.Admin)
            {
                return ServiceResponse<IPagingExecutionResult<GetAllReportResponseDTO>>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));
            }

            var reports = await reportRepository.GetAllAsync();

            if (!reports.Any())
            {
                return ServiceResponse<IPagingExecutionResult<GetAllReportResponseDTO>>.Success(await localizationService.Get(ValidationKeys.NoComplaintsAtTheMoment));
            }

            var query = reports.Select(x => new GetAllReportResponseDTO
            {
                Id = x.Id,
                ReporterUserId = x.ReporterUserId,
                TargetType = x.TargetType,
                ClubId = x.ClubId,
                EventId = x.EventId,
                Reason = x.Reason,
                Description = x.Description,
                Status = x.Status,
                CreatedDate = x.CreatedDate,
                ReviewedAt = x.ReviewedAt,

                TargetDetailRoute = x.TargetType switch
                {
                    ReportTargetType.Event => $"/admin/events/{x.EventId}",
                    ReportTargetType.Club => $"/admin/clubs/{x.ClubId}",
                    _ => null
                }
            });

            var result = await baseEntityRepository.GetPagedResult(
                query,
                pageSize: request.PageSize,
                pageIndex: request.Page,
                ordering: q => q.OrderByDescending(x => x.CreatedDate),
                cancellationToken: cancellationToken
            );

            return ServiceResponse<IPagingExecutionResult<GetAllReportResponseDTO>>.Success(null,result);
        }

    }
}
