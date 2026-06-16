using UniTrack.Domain.Enums;

namespace UniTrack.Domain.Entities
{
    public class User : BaseEntity
    {
        public Guid Id { get; set; }
        public Role Role { get; set; }
        public DateTimeOffset LastLoginDate { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserDetail UserDetail { get; set; }
        public ICollection<EventUser> EventUsers { get; set; }
        public ICollection<UserClub> UserClubs { get; set; } 
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Ban> Bans { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<UserNotification> UserNotifications { get; set; }
        public ICollection<Report> Reports { get; set; }
        public ICollection<EventQuestion> EventQuestions { get; set; }
        public ClubTeam ClubTeam { get; set; }
        public ICollection<OpportunityUser> OpportunityUsers { get; set; }
    }
}
