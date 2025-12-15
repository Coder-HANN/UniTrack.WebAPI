using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniTrack.Domain.Entities
{
    public class ClubTeam : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserDetailId { get; set; }
        public string Title { get; set; }
        public Guid ClubId { get; set; }
        public Club Club { get; set; }
        public UserDetail UserDetail { get; set; }

    }
}
