using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

public class Report : BaseEntity
{
    public Guid Id { get; set; }

    public Guid ReporterUserId { get; set; }
    public User User { get; set; }

    public ReportTargetType TargetType { get; set; }
    public Guid? ClubId { get; set; }
    public Club Club { get; set; }

    // 🔹 Event Report
    public Guid? EventId { get; set; }
    public Event Event { get; set; }

    public ReportReasonType Reason { get; set; }

    public string Description { get; set; }

    public ReportStatus Status { get; set; }
    // Pending | Reviewed | Resolved | Rejected

    public Guid? ReviewedByAdminId { get; set; }
    public DateTimeOffset? ReviewedAt { get; set; }

}
