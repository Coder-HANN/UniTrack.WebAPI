namespace UniTrack.Application.Abstraction.Services.Sheets
{
    /// <summary>
    /// Kullanıcının Google Sheets'e kaydedilecek bilgilerini taşır.
    /// Bu, Handler'ın hazırlayıp Repository'ye gönderdiği veri paketidir.
    /// </summary>
    public class SheetParticipantDTO
    {
        // Sheets'e kaydedilecek alanlar (userDetailRepository'den çekilir)
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public long PhoneNumber { get; set; }
        public string UniversityName { get; set; }
        public string Graduaiton_Date { get; set; }
        public string DepartmentName { get; set; }
        public DateTimeOffset JoinDate { get; set; } = DateTimeOffset.UtcNow; 
        public bool IsJoinedForSponsor { get; set; }
                                                 
    }
}
