using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Report.Command
{
    public class MarkReportAsReviewedCommandHandler : IRequestHandler<MarkReportAsReviewedCommand, ServiceResponse<string>>
    {
        private readonly IReportRepository reportRepository;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;

        public MarkReportAsReviewedCommandHandler(
            IReportRepository reportRepository,
            ICurrentUserServices currentUserServices,
            ILocalizationService localizationService)
        {
            this.reportRepository = reportRepository;
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(MarkReportAsReviewedCommand request,CancellationToken cancellationToken)
        {
            var adminId = currentUserServices.CurrentUser();

            if (adminId == null || currentUserServices.Role() != Role.Admin)
            {
                return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));
            }

            var report = await reportRepository.GetByIdAsync(request.ReportId);

            if (report == null)
            {
                return ServiceResponse<string>.Success(await localizationService.Get(ValidationKeys.ReportNotFound));
            }

            if (report.Status != ReportStatus.Pending)
            {
                return ServiceResponse<string>.Success(await localizationService.Get(ValidationKeys.AlreadyReviewed));
            }

            report.Status = ReportStatus.Reviewed;
            report.ReviewedAt = DateTimeOffset.UtcNow;

            await reportRepository.UpdateAsync(report);

            return ServiceResponse<string>.Success(await localizationService.Get(ValidationKeys.MarkedAsReviewed));
        }
    }
}