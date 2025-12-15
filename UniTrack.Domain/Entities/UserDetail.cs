using UniTrack.Domain.Enums;

namespace UniTrack.Domain.Entities
{
    public class UserDetail : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid UniverstiyId { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public Gender Gender { get; set; }
        public DateOnly BirthDate { get; set; }
        public byte? ProfileImage { get; set; }
        public long PhoneNumber { get; set; } // TO DO: Düzenle,entegre et
        public DateTime Graduaiton_Date { get; set; }  // TO DO: Düzenle,  
        public Guid UserId { get; set; }
        public int CityId { get; set; }
        public User User { get; set; }
        public University University { get; set; }
        public City City { get; set; }
        public ClubTeam ClubTeam { get; set; }
        public bool IsNotified { get; set; } = true; // TO DO : Düzenle
        public string Language { get; set; }
    }
}
