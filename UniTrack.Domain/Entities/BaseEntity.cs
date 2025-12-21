using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniTrack.Domain.Entities
{
    [NotMapped]
    public class BaseEntity
    {
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTimeOffset? DeletedDate { get; set; }
        public bool IsVerified { get; set; }

    }
}
