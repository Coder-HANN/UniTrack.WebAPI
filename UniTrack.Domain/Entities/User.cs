using UniTrack.Domain.Enums;

namespace UniTrack.Domain.Entities
{
    public class User : BaseEntity
    {
        public Guid Id { get; set; }
        public Role Role { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public UserDetail UserDetail { get; set; }
        public ICollection<EventUser> EventUsers { get; set; }
        public ICollection<UserClub> UserClubs { get; set; } 
        public ICollection<Comment> Comments { get; set; }
        public Ban Ban { get; set; }
        public ICollection<Notification> Notification { get; set; }
    }
}
