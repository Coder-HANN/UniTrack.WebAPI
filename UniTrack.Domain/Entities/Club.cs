using UniTrack.Domain.Enums;

namespace UniTrack.Domain.Entities
{
    public class Club : BaseEntity
    {
        public Guid Id { get; set; }
        public Role Role { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public Guid UniversityId { get; set; }
        public string President { get; set; }
        public string PresidentMail { get; set; }
        public string ContectEmail { get; set; }
        public string? Description { get; set; }
        public string? LongDescription { get; set; }
        public string? InstagramLink { get; set; }
        public string? TwitterLink { get; set; }
        public string? WebsiteLink { get; set; }
        public string? LinkedlnLink { get; set; }
        public long Follower { get; set; }
        public DateTimeOffset LastLoginDate { get; set; }
        public byte? Logo { get; set; }
        public byte? CoverImage { get; set; }
        public  Tag Tag { get; set; }
        public int CityId { get; set; }
        public DateOnly ClubCreatedDate { get; set; }
        public ICollection<Event> Events { get; set; }
        public ICollection<UserClub> UserClubs { get; set; } 
        public ICollection<Comment> Comments { get; set; }
        public University University { get; set; }
        public Ban Ban { get; set; }
        public City City { get; set; }
        public ICollection<ClubTeam> ClubTeams { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}
