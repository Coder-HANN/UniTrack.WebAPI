using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniTrack.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public Guid Id { get; set; }
        public int Point { get; set; }
        public string? Description { get; set; }
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public Guid ClubId { get; set; }
        public User User { get; set; }
        public Event Event { get; set; }
        public Club Club { get; set; }
    }
}
