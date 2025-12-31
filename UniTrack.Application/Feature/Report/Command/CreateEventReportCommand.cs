using MediatR;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

public class CreateEventReportCommand : IRequest<ServiceResponse<string>>
{
    public Guid EventId { get; set; }
    public ReportReasonType Reason { get; set; }
    public string Description { get; set; }
}
