using MediatR;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Report
{
    public class CreateClubReportCommand : IRequest<ServiceResponse<string>>
    {
        public Guid ClubId { get; set; }
        public ReportReasonType Reason { get; set; }
        public string Description { get; set; }
    }
}
