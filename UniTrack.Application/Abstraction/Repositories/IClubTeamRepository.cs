using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface IClubTeamRepository : IBaseEntityRepository<ClubTeam>
    {
        Task <ClubTeam>GetClubTeamId(Guid clubTeamId);
        Task <List<ClubTeam>>GetClubTeamsByClubIdAsync(Guid clubId);
    }
}
