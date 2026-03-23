namespace UniTrack.Application.DTOs.ClubTeam
{
    public class ClubTeamResponseDTO
    {
        public Guid ClubTeamId { get; set; }
        public string Title { get; set; }
        public string PersonName { get; set; }
        public string PersonSurname { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}
