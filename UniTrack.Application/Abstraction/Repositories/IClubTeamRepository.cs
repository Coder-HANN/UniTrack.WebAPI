using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IClubTeamRepository : BaseEntityRepository<ClubTeam>
    {
        Task <ClubTeam>GetClubTeamId(Guid clubTeamId);
        Task <List<ClubTeam>>GetClubTeamsByClubIdAsync(Guid clubId);
    }
}
