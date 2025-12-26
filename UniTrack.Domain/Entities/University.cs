namespace UniTrack.Domain.Entities
{
    public class University : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<UserDetail> UserDetails { get; set; }
        public ICollection<Club> Clubs { get; set; }
        public ICollection<Event> Events { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
        public ICollection<Department> Departments { get; set; }
        public ICollection<TargetNotificationUniversity> TargetNotificationUniversities { get; set; }
    }
}
