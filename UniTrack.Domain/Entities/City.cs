namespace UniTrack.Domain.Entities
{
    public class City : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<University> Universities { get; set; }
        public ICollection<UserDetail> UserDetails { get; set; }
        public ICollection<Club> Clubs { get; set; }
        public ICollection<Event> Events { get; set; }

    }
}
