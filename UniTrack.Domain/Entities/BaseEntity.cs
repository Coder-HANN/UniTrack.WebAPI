using System.ComponentModel.DataAnnotations.Schema;

namespace UniTrack.Domain.Entities
{
    [NotMapped]
    public class BaseEntity
    {
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedDate { get; set; }
        public bool IsVerified { get; set; }

    }
}
