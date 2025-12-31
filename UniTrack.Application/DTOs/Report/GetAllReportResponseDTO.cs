using UniTrack.Domain.Enums;

namespace UniTrack.Application.DTOs.Report
{
    public class GetAllReportResponseDTO
    {
        public Guid Id { get; set; }
        public Guid ReporterUserId { get; set; }
        public ReportTargetType TargetType { get; set; }
        public Guid? ClubId { get; set; }
        public Guid? EventId { get; set; }
        public ReportReasonType Reason { get; set; }
        public string Description { get; set; }
        public ReportStatus Status { get; set; }

        public string TargetDetailRoute { get; set; }
        public DateTimeOffset? ReviewedAt { get; set; }
        public DateTimeOffset? CreatedDate { get;set; }
    }
}
