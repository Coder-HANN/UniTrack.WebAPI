using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Report;
using UniTrack.Domain.Enums;

public class CreateClubReportCommandHandler : IRequestHandler<CreateClubReportCommand, ServiceResponse<string>>
{
    private readonly IReportRepository reportRepository;
    private readonly IClubRepository clubRepository;
    private readonly ICurrentUserServices currentUser;
    private readonly ILocalizationService localizationService;

    public CreateClubReportCommandHandler(
        IReportRepository reportRepository,
        IClubRepository clubRepository,
        ICurrentUserServices currentUser,
        ILocalizationService localizationService)
    {
        this.reportRepository = reportRepository;
        this.clubRepository = clubRepository;
        this.currentUser = currentUser;
        this.localizationService = localizationService;
    }

    public async Task<ServiceResponse<string>> Handle(CreateClubReportCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.CurrentUser();
        if (userId == null)
        {
            return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));
        }

        var eventEntity = await clubRepository.GetByIdAsync(request.ClubId);

        if (eventEntity is null)
            return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.EventNotFound));

        var alreadyReported = await reportRepository.GetReportClubAsync(userId.Value, request.ClubId);

        if (alreadyReported)
            return ServiceResponse<string>.Fail("You already reported this event");

        var report = new Report
        {
            Id = Guid.NewGuid(),
            ReporterUserId = userId.Value,
            TargetType = ReportTargetType.Event,
            ClubId = request.ClubId,
            Reason = request.Reason,
            Description = request.Description,
            Status = ReportStatus.Pending
        };

        await reportRepository.AddAsync(report);

        return ServiceResponse<string>.Success(null, await localizationService.Get(ValidationKeys.OperationSuccessful));
    }
}
