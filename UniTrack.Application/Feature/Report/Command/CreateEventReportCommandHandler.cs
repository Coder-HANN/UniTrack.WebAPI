using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

public class CreateEventReportCommandHandler: IRequestHandler<CreateEventReportCommand, ServiceResponse<string>>
{
    private readonly IReportRepository reportRepository;
    private readonly IEventRepository eventRepository;
    private readonly ICurrentUserServices currentUser;
    private readonly ILocalizationService localizationService;

    public CreateEventReportCommandHandler(
        IReportRepository reportRepository,
        IEventRepository eventRepository,
        ICurrentUserServices currentUser,
        ILocalizationService localizationService)
    {
        this.reportRepository = reportRepository;
        this.eventRepository = eventRepository;
        this.currentUser = currentUser;
        this.localizationService = localizationService;
    }

    public async Task<ServiceResponse<string>> Handle(CreateEventReportCommand request,CancellationToken cancellationToken)
    {
        var userId = currentUser.CurrentUser();
        if (userId == null)
        {
            return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));
        }

        var eventEntity = await eventRepository.GetByIdAsync(request.EventId);
        if (eventEntity is null)
            return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.EventNotFound));

        var alreadyReported = await reportRepository.GetReportEventAsync(userId.Value, request.EventId);

        if (alreadyReported)
            return ServiceResponse<string>.Fail(await localizationService.Get(ValidationKeys.EventAlreadyReported));

        var report = new Report
        {
            Id = Guid.NewGuid(),
            ReporterUserId = userId.Value,
            TargetType = ReportTargetType.Event,
            EventId = request.EventId,
            Reason = request.Reason,
            Description = request.Description,
            Status = ReportStatus.Pending
        };

        await reportRepository.AddAsync(report);

        return ServiceResponse<string>.Success(null, await localizationService.Get(ValidationKeys.OperationSuccessful));
    }
}
