using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Report.Command
{
    public class MarkReportAsReviewedCommand : IRequest<ServiceResponse<string>>
    {
        public Guid ReportId { get; set; }
    }

}
