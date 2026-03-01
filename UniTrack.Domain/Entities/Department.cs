namespace UniTrack.Domain.Entities
{
    public class Department : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<UserDetail> UserDetails { get; set; }
        public ICollection<TargetNotificationDepartment> Departments { get; set; }
    }
}
